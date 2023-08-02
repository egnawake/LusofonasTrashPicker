using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// View class. Renders an instance of Trash Picker in Unity.
/// </summary>
public class GameView : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Distance between cell objects.")]
    private float gridSpacing = 1f;

    [SerializeField]
    [Tooltip("Time the robot takes moving from one cell to another.")]
    private float robotMoveDuration = 0.5f;

    [SerializeField]
    [Tooltip("Time the robot takes wrapping around the grid.")]
    private float robotWrapDuration = 1f;

    [SerializeField]
    [Tooltip("Animation curve for robot movement.")]
    private AnimationCurve robotMoveCurve;

    [SerializeField]
    [Tooltip("Animation curve for when robot moves against a wall.")]
    private AnimationCurve robotBumpCurve;

    [SerializeField]
    [Tooltip("Animation curve for when player skips a turn.")]
    private AnimationCurve robotSkipTurnCurve;

    [SerializeField]
    [Tooltip("Duration of the robot collect trash animation.")]
    private float robotCollectTrashDuration = 0.5f;

    [SerializeField]
    [Tooltip("Animation curve for when player picks up trash.")]
    private AnimationCurve robotCollectTrashCurve;

    [SerializeField]
    [Tooltip("Duration of the robot spawn animation.")]
    private float robotSpawnDuration = 0.5f;

    [SerializeField]
    [Tooltip("Animation curve for the robot spawn animation.")]
    private AnimationCurve robotSpawnCurve;

    [SerializeField] private CellView cellPrefab;
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject trashPickupEffectPrefab;


    private CellView[,] cellObjects;
    private GameObject robot;
    private HUD hud;

    private TrashPickerGame game;
    private RobotAction lastAction;

    private bool animating = false;


    /// <summary>
    /// Indicates whether there is an animation in course.
    /// </summary>
    public bool Animating => animating;

    /// <summary>
    /// Initializes the view with a reference to a Trash Picker game.
    /// This method should only be called once.
    /// </summary>
    ///
    /// <param name="game">The game instance this view will render.</param>
    public void Setup(TrashPickerGame game)
    {
        // Setup should only be called once
        if (this.game != null)
            return;

        this.game = game;
        InitializeCellGrid();
        InitializeRobot();
        hud = GetComponentInChildren<HUD>();

        DrawCells();

        hud.ShowScore(game.Score);
        hud.ShowTurn(game.Turn, game.MaxTurns);
    }

    /// <summary>
    /// Renders the game according to the current game state.
    /// </summary>
    ///
    /// <param name="lastAction">
    /// The last action performed in the game.
    /// </param>
    /// <param name="success">
    /// The success of the last action.
    /// </param>
    /// <param name="lastPosition">
    /// The position of the robot right before the last action.
    /// </param>
    /// <param name="targetPosition">
    /// The last position the robot tried to move to.
    /// </param>
    public void Draw(RobotAction lastAction, bool success, Position lastPosition,
        Position targetPosition)
    {
        DrawCells();
        DrawRobot(lastAction, success, lastPosition, targetPosition);
        DrawTrashEffect(lastAction, success);

        hud.ShowScore(game.Score);
        hud.ShowTurn(game.Turn, game.MaxTurns);
    }

    private void InitializeCellGrid()
    {
        cellObjects = new CellView[game.Rows, game.Cols];

        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                CellView cellObject = Instantiate(cellPrefab, transform);

                cellObject.name = $"CellObject({i}, {j})";
                cellObject.transform.position = CellToWorldPosition(
                    new Position(i, j));

                cellObjects[i, j] = cellObject;
            }
        }
    }

    private void InitializeRobot()
    {
        // Instantiate robot as child of game view
        robot = Instantiate(robotPrefab, transform);

        // Make robot face camera
        robot.transform.rotation = Quaternion.LookRotation(-Vector3.forward);

        // Animate spawn
        Vector3 firstCellPosition = CellToWorldPosition(new Position(0, 0));
        Vector3 start = firstCellPosition + Vector3.up * 5f;
        Vector3 end = firstCellPosition;
        StartCoroutine(AnimateRobotMovement(start, end, robotSpawnCurve,
            robotSpawnDuration));
    }

    private void DrawCells()
    {
        // Update cell states
        for (int i = 0; i < game.Rows; i++)
        {
            for (int j = 0; j < game.Cols; j++)
            {
                cellObjects[i, j].SetState(game.CellAt(new Position(i, j)));
            }
        }
    }

    private void DrawRobot(RobotAction lastAction, bool success,
        Position lastPosition, Position targetPosition)
    {
        if (animating) return;

        if (lastAction == RobotAction.MoveNorth
            || lastAction == RobotAction.MoveEast
            || lastAction == RobotAction.MoveSouth
            || lastAction == RobotAction.MoveWest
            || lastAction == RobotAction.MoveRandom)
        {
            Vector3 start = CellToWorldPosition(lastPosition);
            Vector3 end = CellToWorldPosition(targetPosition);

            // If movement was successful, animate robot from last position
            // to new position
            if (success)
            {
                Position diff = targetPosition - lastPosition;
                bool wrapped = Mathf.Abs(diff.Row) > 1 || Mathf.Abs(diff.Col) > 1;

                if (wrapped)
                {
                    robot.transform.rotation = Quaternion.LookRotation(start - end);
                    StartCoroutine(AnimateRobotWrap(start, end, robotMoveCurve,
                        robotWrapDuration));
                }
                else
                {
                    robot.transform.rotation = Quaternion.LookRotation(
                        end - start);
                    StartCoroutine(AnimateRobotMovement(start, end, robotMoveCurve,
                        robotMoveDuration));
                }
            }
            // If movement was not successful, animate robot bumping into a wall
            else
            {
                robot.transform.rotation = Quaternion.LookRotation(
                    end - start);
                StartCoroutine(AnimateRobotMovement(start, end, robotBumpCurve,
                    robotMoveDuration));
            }
        }
        else if (lastAction == RobotAction.SkipTurn)
        {
            // Perform skip turn animation
            Vector3 start = CellToWorldPosition(game.RobotPosition);
            Vector3 end = start + Vector3.up * 1.2f;
            StartCoroutine(AnimateRobotMovement(start, end, robotSkipTurnCurve,
                robotMoveDuration));
        }
        else if (lastAction == RobotAction.CollectTrash)
        {
            // Perform collect trash animation
            Vector3 start = CellToWorldPosition(game.RobotPosition);
            Vector3 end = start + robot.transform.rotation * Vector3.right * 1.2f;
            StartCoroutine(AnimateRobotMovement(start, end, robotCollectTrashCurve,
                robotCollectTrashDuration));
        }
    }

    private void DrawTrashEffect(RobotAction lastAction, bool success)
    {
        // Play trash effect if trash was collected
        if (lastAction == RobotAction.CollectTrash && success)
        {
            Instantiate(trashPickupEffectPrefab,
                CellToWorldPosition(game.RobotPosition),
                Quaternion.identity);
        }
    }

    private Vector3 CellToWorldPosition(Position pos)
    {
        Vector3 position = new Vector3(pos.Col * gridSpacing, 0,
            -(pos.Row * gridSpacing));

        // Offset to center
        float rowLength = gridSpacing * (game.Cols - 1);
        float colLength = gridSpacing * (game.Rows - 1);
        Vector3 offset = new Vector3(
            // Subtracting 1 unit of spacing from length (HARDCODED AT THE MOMENT)
            -(rowLength - 0.2f) / 2, 0, (colLength - 0.2f) / 2);

        return transform.TransformPoint(position + offset);
    }

    private IEnumerator AnimateRobotWrap(Vector3 start, Vector3 end,
        AnimationCurve curve, float duration)
    {
        Vector3 exitStart = start;

        Vector3 offset = (end - start).normalized * 1.2f;
        Vector3 exitEnd = start - offset;

        Vector3 enterEnd = end;
        Vector3 enterStart = end + offset;

        yield return AnimateRobotMovement(exitStart, exitEnd, curve, duration / 2);
        yield return AnimateRobotMovement(enterStart, enterEnd, curve, duration / 2);
    }

    private IEnumerator AnimateRobotMovement(Vector3 start, Vector3 end,
        AnimationCurve curve, float duration)
    {
        animating = true;

        float timer = 0;

        while (timer < duration)
        {
            // Model Lerp time input according to curve
            float pct = curve.Evaluate(timer / duration);
            robot.transform.position = Vector3.LerpUnclamped(start, end, pct);

            timer += Time.deltaTime;

            yield return null;
        }

        animating = false;
    }
}
