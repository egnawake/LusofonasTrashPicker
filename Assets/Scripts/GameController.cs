using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using NaiveBayes;

/// <summary>
/// Controller class for processing game input, updating the game model and
/// communicating with the view.
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int gridRows = 5;

    [SerializeField]
    [Min(1)]
    private int gridColumns = 5;

    [SerializeField]
    [Min(1)]
    private int maxTurns = 20;

    [SerializeField]
    [Tooltip("Probability that a cell will have trash on it when the map is created.")]
    [Range(0, 1f)]
    private float trashSpawnChance = 0.4f;

    [SerializeField] private string strategyPath;

    [SerializeField] private ScoreConfigSO scoreConfigSO;

    [SerializeField] private GameView gameViewPrefab;
    [SerializeField] private MainMenuController mainMenuControllerPrefab;

    // Game over screen fields
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gameOverHighScoreDisplay;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text gameOverHighScoreText;
    [SerializeField] private Button gameOverReturnButton;

    // Camera animation fields
    [SerializeField] private float cameraSlideDuration = 1f;
    [SerializeField] private AnimationCurve cameraSlideCurve;
    [SerializeField] private Transform cameraSlideInStart;
    [SerializeField] private Transform cameraSlideInEnd;


    private Camera mainCamera;
    private CameraMovement cameraMovement;
    private TrashPickerGame game;
    private MainMenuController mainMenuController;
    private GameView gameView;

    private List<HighScore> highScores;

    private bool aiPaused = false;
    private string playerType = "human";
    private NaiveBayesClassifier nbClassifier;
    private Attrib centerCell, northCell, eastCell, southCell, westCell;

    private RobotAction[] strategy;
    private SessionLogger sessionLogger;


    private void Start()
    {
        AttachButtonListeners();

        mainCamera = Camera.main;
        cameraMovement = mainCamera.GetComponent<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        highScores = new List<HighScore>();

        InitializeAI();

        InitializeGAPlayer();

        sessionLogger = new SessionLogger();

        mainMenuController = GetComponent<MainMenuController>();
        mainMenuController.GameStarted += StartGame;
        mainMenuController.HighScores = highScores;
    }

    private void InitializeAI()
    {
        // Set up attributes with possible cell states
        string[] attribValues = new string[]
        {
            "Empty",
            "Trash",
            "Wall"
        };

        centerCell = new Attrib("centerCell", attribValues);
        northCell = new Attrib("northCell", attribValues);
        eastCell = new Attrib("eastCell", attribValues);
        southCell = new Attrib("southCell", attribValues);
        westCell = new Attrib("westCell", attribValues);

        // Discard RobotAction.None before initializing NBC
        string[] allActions = Enum.GetNames(typeof(RobotAction));
        string[] validActions = new string[allActions.Length - 1];
        Array.Copy(allActions, 1, validActions, 0, allActions.Length - 1);

        // Initialize NBC
        nbClassifier = new NaiveBayesClassifier(validActions,
            new Attrib[]
            {
                centerCell,
                northCell,
                eastCell,
                southCell,
                westCell
            });
    }

    private void InitializeGAPlayer()
    {
        string s = File.ReadAllText(
            Path.Combine(Application.persistentDataPath, strategyPath, "strategy.txt"));

        strategy = s.Split(',')
            .Select((string s) => (RobotAction)Enum.Parse(typeof(RobotAction), s))
            .ToArray();
    }

    private void PlayerAction(bool draw = true)
    {
        RobotAction action = RobotAction.None;

        if (Input.GetButtonDown("Skip Turn"))
        {
            action = RobotAction.SkipTurn;
        }
        else if (Input.GetButtonDown("Collect"))
        {
            action = RobotAction.CollectTrash;
        }
        else if (Input.GetButtonDown("Up"))
        {
            action = RobotAction.MoveNorth;
        }
        else if (Input.GetButtonDown("Right"))
        {
            action = RobotAction.MoveEast;
        }
        else if (Input.GetButtonDown("Down"))
        {
            action = RobotAction.MoveSouth;
        }
        else if (Input.GetButtonDown("Left"))
        {
            action = RobotAction.MoveWest;
        }
        else if (Input.GetButtonDown("Move Random"))
        {
            action = RobotAction.MoveRandom;
        }

        // If there was player input, add new data to the NBC and do the action
        if (action != RobotAction.None)
        {
            nbClassifier.Update(action.ToString(), new Dictionary<Attrib, string>()
            {
                { centerCell, game.CellAt(game.RobotPosition).ToString() },
                { northCell, game.CellAt(game.RobotPosition + new Position(-1, 0)).ToString() },
                { eastCell, game.CellAt(game.RobotPosition + new Position(0, 1)).ToString() },
                { southCell, game.CellAt(game.RobotPosition + new Position(1, 0)).ToString() },
                { westCell, game.CellAt(game.RobotPosition + new Position(0, -1)).ToString() }
            });

            DoAction(action, draw);
        }
    }

    private void AIAction(bool draw = true)
    {
        RobotAction action = RobotAction.None;

        // Ask NBC for a prediction
        string prediction = nbClassifier.Predict(new Dictionary<Attrib, string>
        {
            { centerCell, game.CellAt(game.RobotPosition).ToString() },
            { northCell, game.CellAt(game.RobotPosition + new Position(-1, 0)).ToString() },
            { eastCell, game.CellAt(game.RobotPosition + new Position(0, 1)).ToString() },
            { southCell, game.CellAt(game.RobotPosition + new Position(1, 0)).ToString() },
            { westCell, game.CellAt(game.RobotPosition + new Position(0, -1)).ToString() }
        });

        // Select random action if prediction gave no result
        if (prediction == "")
        {
            string[] actions = Enum.GetNames(typeof(RobotAction));
            int index = UnityEngine.Random.Range(1, actions.Length);
            prediction = actions[index];
        }

        // Convert prediction to RobotAction
        Enum.TryParse<RobotAction>(prediction, out action);

        DoAction(action, draw);
    }

    private void GAAction()
    {
        int actionIndex = GetGAStrategyKey(game);

        RobotAction action = strategy[actionIndex];

        IList<CellState> states = new List<CellState>
        {
            game.CellAt(game.RobotPosition),
            game.CellAt(game.RobotPosition + new Position(-1, 0)),
            game.CellAt(game.RobotPosition + new Position(0, 1)),
            game.CellAt(game.RobotPosition + new Position(1, 0)),
            game.CellAt(game.RobotPosition + new Position(0, -1))
        };

        Debug.Log($"Action at {actionIndex} - {action} - {string.Join(' ', states)}");

        DoAction(action);
    }

    private void DoAction(RobotAction action, bool draw = true)
    {
        Position lastPosition = game.RobotPosition;
        bool actionSuccess = false;

        if (action == RobotAction.MoveNorth)
        {
            actionSuccess = game.MoveRobot(Direction.North);
        }
        else if (action == RobotAction.MoveEast)
        {
            actionSuccess = game.MoveRobot(Direction.East);
        }
        else if (action == RobotAction.MoveSouth)
        {
            actionSuccess = game.MoveRobot(Direction.South);
        }
        else if (action == RobotAction.MoveWest)
        {
            actionSuccess = game.MoveRobot(Direction.West);
        }
        else if (action == RobotAction.MoveRandom)
        {
            Direction dir = (Direction)UnityEngine.Random.Range(0,
                Enum.GetNames(typeof(Direction)).Length);
            actionSuccess = game.MoveRobot(dir);
        }
        else if (action == RobotAction.SkipTurn)
        {
            game.SkipTurn();
        }
        else if (action == RobotAction.CollectTrash)
        {
            actionSuccess = game.CollectTrash();
        }

        if (draw)
        {
            gameView.Draw(action, actionSuccess, lastPosition, game.TargetPosition);
            if (game.GameOver) HandleGameOver();
        }

    }

    private void StartHumanGame()
    {
        StartGame("human");
    }

    private void StartAIGame()
    {
        StartGame("bayes");
    }

    private void StartGAGame()
    {
        StartGame("genetic");
    }

    private void StartGame(string playerType)
    {
        // Initialize game with parameters from the Unity inspector
        game = new TrashPickerGame(gridRows, gridColumns, trashSpawnChance,
            scoreConfigSO.ToScoreConfig());

        // Instantiate a view object and pass it the game
        gameView = Instantiate(gameViewPrefab);
        gameView.Setup(game);

        this.playerType = playerType;

        // Activate camera movement
        if (cameraMovement != null)
        {
            cameraMovement.enabled = true;
        }

        // Camera enter animation
        StartCoroutine(CameraSlideIn());

        // Start the coroutine that processes input and updates the game
        StartCoroutine(PlayGame());
    }

    private void PlayQuickBayesGame()
    {
        game = new TrashPickerGame(gridRows, gridColumns, trashSpawnChance,
            scoreConfigSO.ToScoreConfig());

        this.playerType = "bayes";

        while (!game.GameOver)
        {
            AIAction(false);
        }

        sessionLogger.Log(playerType, game);
    }

    private void HandleGameOver()
    {
        bool showHighScore = false;

        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        gameOverScreen.SetActive(true);
        gameOverScoreText.text = game.Score.ToString();

        int placement = AddHighScore(new HighScore(game.Score, playerType == "bayes"));

        if (placement > 0)
        {
            showHighScore = true;
            gameOverHighScoreText.text = $"#{placement}";
        }

        gameOverHighScoreDisplay.SetActive(showHighScore);
    }

    private int AddHighScore(HighScore highScore)
    {
        int placement = 0;
        int index = -1;

        // If this score is greater than a score on the table,
        // store that index and insert the new score there
        for (int i = 0; i < highScores.Count; i++)
        {
            if (highScore.Score > highScores[i].Score)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            highScores.Insert(index, highScore);
            placement = index + 1;
        }

        // If score did not beat any previous score and
        // there is space in the scores list, add score
        if (placement == 0 && highScores.Count < 6)
        {
            highScores.Add(highScore);
            placement = highScores.Count;
        }

        // Limit scores list size
        if (highScores.Count > 6)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        return placement;
    }

    private void OpenMainMenu()
    {
        gameOverScreen.SetActive(false);

        if (gameView != null)
        {
            Destroy(gameView.gameObject);
        }

        mainMenuController.Open();
    }

    private void AttachButtonListeners()
    {
        gameOverReturnButton.onClick.AddListener(OpenMainMenu);
    }

    private IEnumerator PlayGame()
    {
        while (!game.GameOver)
        {
            if (!gameView.Animating)
            {
                if (playerType == "human")
                {
                    PlayerAction();
                }
                else if (!aiPaused && playerType == "bayes")
                {
                    AIAction();

                    aiPaused = true;
                    StartCoroutine(AIPauseTimer());
                }
                else if (!aiPaused && playerType == "genetic")
                {
                    GAAction();

                    aiPaused = true;
                    StartCoroutine(AIPauseTimer());
                }
            }

            yield return null;
        }

        sessionLogger.Log(playerType, game);

        if (playerType == "human")
        {
            PlayQuickBayesGame();
        }
    }

    private IEnumerator AIPauseTimer()
    {
        yield return new WaitForSeconds(0.5f);
        aiPaused = false;
    }

    private IEnumerator CameraSlideIn()
    {
        float timer = 0;

        while (timer < cameraSlideDuration)
        {
            float pct = cameraSlideCurve.Evaluate(timer / cameraSlideDuration);
            Vector3 position = Vector3.Lerp(cameraSlideInStart.position,
                cameraSlideInEnd.position, pct);
            mainCamera.transform.position = position;

            timer += Time.deltaTime;

            yield return null;
        }
    }

    private int GetGAStrategyKey(TrashPickerGame game)
    {
        IList<CellState> states = new List<CellState>
        {
            game.CellAt(game.RobotPosition),
            game.CellAt(game.RobotPosition + new Position(-1, 0)),
            game.CellAt(game.RobotPosition + new Position(0, 1)),
            game.CellAt(game.RobotPosition + new Position(1, 0)),
            game.CellAt(game.RobotPosition + new Position(0, -1))
        };

        int key = 0;
        for (int i = 0; i < states.Count; i++)
        {
            key = key + (int)states[i] * (int)Math.Pow(3, i);
        }

        return key;
    }
}
