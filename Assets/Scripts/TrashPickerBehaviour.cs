using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaiveBayes;

public class TrashPickerBehaviour : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Number of rows in the grid.")]
    private int rows = 5;

    [SerializeField]
    [Tooltip("Number of columns in the grid.")]
    private int cols = 5;

    [SerializeField]
    [Tooltip("Amount of moves the robot can make.")]
    private int maxTurns = 20;

    [SerializeField]
    [Tooltip("Probability that a cell will have trash on it.")]
    [Range(0, 1f)]
    private double trashSpawnChance = 0.4;

    [SerializeField]
    [Tooltip("Distance between cell objects.")]
    private float gridSpacing = 3f;

    [SerializeField] private float robotMoveTime = 0.5f;
    [SerializeField] private AnimationCurve robotMoveCurve;

    [SerializeField] private CellView cellPrefab;
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject trashPickupEffectPrefab;

    private TrashPickerGame game;
    private CellView[,] cellObjects;
    private GameObject robot;
    private bool animating = false;

    private bool aiEnabled = false;
    private bool aiPaused = false;

    private NaiveBayesClassifier nbClassifier;
    private Attrib centerCell, northCell, eastCell, southCell, westCell;

    public ActionEvent OnAction => onAction;

    private void Awake()
    {
        onAction = new ActionEvent();
        InitializeAI();
    }

    private void Start()
    {
        // Instantiate game
        game = new TrashPickerGame(rows, cols, maxTurns, trashSpawnChance);

        cellObjects = new CellView[rows, cols];

        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                CellView cellObject = Instantiate(cellPrefab, transform);

                cellObject.name = $"CellObject({i}, {j})";
                cellObject.transform.position = CellToWorldPosition(new Position(i, j));

                cellObjects[i, j] = cellObject;
            }
        }

        robot = Instantiate(robotPrefab);
        robot.transform.position = CellToWorldPosition(new Position(0, 0));
        robot.transform.rotation = Quaternion.LookRotation(-Vector3.forward);

        UpdateView();
    }

    private void Update()
    {
        if (animating) return;

        if (!aiEnabled)
        {
            DoActionPlayer();
        }
        else if (!aiPaused)
        {
            DoActionAI();
        }

        if (Input.GetButtonDown("DEBUG Enable AI"))
        {
            aiEnabled = !aiEnabled;
        }

        if (Input.GetButtonDown("DEBUG New Game"))
        {
            game = new TrashPickerGame(rows, cols, maxTurns, trashSpawnChance);
            UpdateView();
        }
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

        string[] actionNames = Enum.GetNames(typeof(RobotAction));
        nbClassifier = new NaiveBayesClassifier(actionNames,
            new Attrib[]
            {
                centerCell,
                northCell,
                eastCell,
                southCell,
                westCell
            });
    }

    private void DoActionPlayer()
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

    private void DoActionAI()
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
        Debug.Log($"AI predicted [{prediction}]");

        Enum.TryParse<RobotAction>(prediction, out action);

        DoAction(action);

        aiPaused = true;
        StartCoroutine(AIPauseTimer());
    }

    private IEnumerator AIPauseTimer()
    {
        yield return new WaitForSeconds(0.5f);
        aiPaused = false;
    }

    private void DoAction(RobotAction action)
    {
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
            game.MoveRobot((Direction)UnityEngine.Random.Range(0, 4));
        }
        else if (action == RobotAction.SkipTurn)
        {
            game.SkipTurn();
        }
        else if (action == RobotAction.CollectTrash)
        {
            bool collected = game.CollectTrash();
            if (collected)
            {
                Instantiate(trashPickupEffectPrefab,
                    CellToWorldPosition(game.RobotPosition),
                    Quaternion.identity);
            }
        }

        UpdateView();
        onAction.Invoke(game.Turn, game.MaxTurns, game.Score);
    }

    private Vector3 CellToWorldPosition(Position pos)
    {
        Vector3 position = new Vector3(pos.Col * gridSpacing, 0,
            -(pos.Row * gridSpacing));

        // Offset to center
        float rowLength = gridSpacing * (game.Cols - 1);
        float colLength = gridSpacing * (game.Rows - 1);
        Vector3 offset = new Vector3(-rowLength / 2, 0, colLength / 2);

        return position + offset;
    }

    private void UpdateView()
    {
        // Update cell states
        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                cellObjects[i, j].SetState(game.CellAt(new Position(i, j)));
            }
        }

        // Update robot position and rotation
        Vector3 currentPos = robot.transform.position;
        Vector3 targetPos = CellToWorldPosition(game.RobotPosition);
        Vector3 disp = targetPos - currentPos;
        if (disp.magnitude > 0.01f)
        {
            robot.transform.rotation = Quaternion.LookRotation(disp);
            StartCoroutine(AnimateRobot(currentPos, targetPos));
        }
    }

    private IEnumerator AnimateRobot(Vector3 start, Vector3 end)
    {
        animating = true;

        float timer = 0;

        while (timer < robotMoveTime)
        {
            float pct = robotMoveCurve.Evaluate(timer / robotMoveTime);
            robot.transform.position = Vector3.LerpUnclamped(start, end, pct);
            timer += Time.deltaTime;
            yield return null;
        }

        animating = false;
    }

    private ActionEvent onAction;
}
