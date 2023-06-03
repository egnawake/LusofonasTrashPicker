using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using NaiveBayes;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private int gridRows = 5;

    [SerializeField]
    private int gridColumns = 5;

    [SerializeField]
    private int maxTurns = 20;

    [SerializeField]
    [Tooltip("Probability that a cell will have trash on it when the map is created.")]
    [Range(0, 1f)]
    private float trashSpawnChance = 0.4f;

    [SerializeField] private GameView gameViewPrefab;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject highScoresScreen;
    [SerializeField] private GameObject gameOverHighScoreDisplay;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text gameOverHighScoreText;

    [SerializeField] private TMP_Text[] highScoreFields;

    [SerializeField] private Button newHumanGameButton;
    [SerializeField] private Button newAIGameButton;
    [SerializeField] private Button highScoresButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button gameOverReturnButton;
    [SerializeField] private Button highScoresReturnButton;

    [SerializeField] private float cameraSlideDuration = 1f;
    [SerializeField] private AnimationCurve cameraSlideCurve;
    [SerializeField] private Transform cameraSlideInStart;
    [SerializeField] private Transform cameraSlideInEnd;

    private List<int> highScores;
    private Camera mainCamera;
    private GameView gameView;
    private TrashPickerGame game;
    private bool isAI = false;
    private bool aiPaused = false;

    private CameraMovement cameraMovement;

    private NaiveBayesClassifier nbClassifier;
    private Attrib centerCell, northCell, eastCell, southCell, westCell;


    private void Start()
    {
        AttachButtonListeners();

        mainCamera = Camera.main;
        cameraMovement = mainCamera.GetComponent<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        highScores = new List<int>();

        InitializeAI();
    }

    private void InitializeAI()
    {
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

    private void PlayerAction()
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

            DoAction(action);
        }
    }

    private void AIAction()
    {
        RobotAction action = RobotAction.None;

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

        Enum.TryParse<RobotAction>(prediction, out action);

        DoAction(action);

        aiPaused = true;
        StartCoroutine(AIPauseTimer());
    }

    private void DoAction(RobotAction action)
    {
        Position lastPosition = game.RobotPosition;
        bool actionSuccess = false;

        if (action == RobotAction.MoveNorth)
        {
            game.MoveRobot(Direction.North);
        }
        else if (action == RobotAction.MoveEast)
        {
            game.MoveRobot(Direction.East);
        }
        else if (action == RobotAction.MoveSouth)
        {
            game.MoveRobot(Direction.South);
        }
        else if (action == RobotAction.MoveWest)
        {
            game.MoveRobot(Direction.West);
        }
        else if (action == RobotAction.MoveRandom)
        {
            game.MoveRobot((Direction)UnityEngine.Random.Range(0,
                Enum.GetNames(typeof(Direction)).Length));
        }
        else if (action == RobotAction.SkipTurn)
        {
            game.SkipTurn();
        }
        else if (action == RobotAction.CollectTrash)
        {
            actionSuccess = game.CollectTrash();
        }

        gameView.Draw(action, actionSuccess, lastPosition);

        if (game.GameOver) HandleGameOver();
    }

    private void StartHumanGame()
    {
        StartGame(false);
    }

    private void StartAIGame()
    {
        StartGame(true);
    }

    private void StartGame(bool isAI)
    {
        mainMenu.SetActive(false);

        // Initialize game with parameters from Unity
        game = new TrashPickerGame(gridRows, gridColumns, maxTurns,
            trashSpawnChance);

        gameView = Instantiate(gameViewPrefab);
        gameView.Setup(game);

        this.isAI = isAI;

        if (cameraMovement != null)
        {
            cameraMovement.enabled = true;
        }

        StartCoroutine(CameraSlideIn());
        StartCoroutine(PlayGame());
    }

    private void HandleGameOver()
    {
        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        gameOverScreen.SetActive(true);

        gameOverScoreText.text = game.Score.ToString();

        int placement = AddHighScore(game.Score);
        bool showHighScore = false;

        if (placement > 0)
        {
            showHighScore = true;
            gameOverHighScoreText.text = $"#{placement}";
        }

        gameOverHighScoreDisplay.SetActive(showHighScore);
    }

    private void ShowHighScores()
    {
        mainMenu.SetActive(false);

        highScoresScreen.SetActive(true);
    }

    private int AddHighScore(int score)
    {
        int placement = 0;
        int index = -1;

        // If this score is greater than a score on the table,
        // store that index and insert the new score there
        for (int i = 0; i < highScores.Count; i++)
        {
            if (score > highScores[i])
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            highScores.Insert(index, score);
            placement = index + 1;
        }

        // If score did not beat any previous score and
        // there is space in the scores list, add score
        if (placement == 0 && highScores.Count < 6)
        {
            highScores.Add(score);
            placement = highScores.Count;
        }

        // Limit scores list size
        if (highScores.Count > 6)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        // Update view with current score list
        for (int i = 0; i < highScores.Count; i++)
        {
            highScoreFields[i].text = highScores[i].ToString();
        }

        return placement;
    }

    private void OpenMainMenu()
    {
        gameOverScreen.SetActive(false);
        highScoresScreen.SetActive(false);

        if (gameView != null)
        {
            Destroy(gameView.gameObject);
        }

        mainMenu.SetActive(true);
    }

    private void AttachButtonListeners()
    {
        newHumanGameButton.onClick.AddListener(StartHumanGame);
        newAIGameButton.onClick.AddListener(StartAIGame);
        highScoresButton.onClick.AddListener(ShowHighScores);
        gameOverReturnButton.onClick.AddListener(OpenMainMenu);
        highScoresReturnButton.onClick.AddListener(OpenMainMenu);
        quitButton.onClick.AddListener(Quit);
    }

    private void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator PlayGame()
    {
        while (!game.GameOver)
        {
            if (!gameView.Animating)
            {
                if (!isAI)
                {
                    PlayerAction();
                }
                else if (!aiPaused)
                {
                    AIAction();
                }
            }

            yield return null;
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
}
