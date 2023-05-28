using UnityEngine;
using System.Collections;

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

    private TrashPickerGame game;
    private CellView[,] cellObjects;
    private GameObject robot;
    private bool animating = false;

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
        if (!animating)
            DoAction();
    }

    private void DoAction()
    {
        if (Input.GetButtonDown("Skip Turn"))
        {
            game.SkipTurn();
            UpdateView();
        }
        else if (Input.GetButtonDown("Collect"))
        {
            game.CollectTrash();
            UpdateView();
        }
        else if (Input.GetButtonDown("Up"))
        {
            game.MoveRobot(Direction.North);
            UpdateView();
        }
        else if (Input.GetButtonDown("Right"))
        {
            game.MoveRobot(Direction.East);
            UpdateView();
        }
        else if (Input.GetButtonDown("Down"))
        {
            game.MoveRobot(Direction.South);
            UpdateView();
        }
        else if (Input.GetButtonDown("Left"))
        {
            game.MoveRobot(Direction.West);
            UpdateView();
        }
        else if (Input.GetButtonDown("Move Random"))
        {
            game.MoveRobot((Direction)Random.Range(0, 4));
            UpdateView();
        }
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
        if (disp.magnitude > 0.001f)
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
}
