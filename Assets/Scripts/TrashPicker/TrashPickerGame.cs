using System;
using System.Collections.Generic;

/// <summary>
/// Trash Picker game engine.
/// </summary>
public class TrashPickerGame
{
    private readonly InternalCellState[,] grid;
    private readonly int maxTurns;
    private readonly Random rng;

    private Position robotPosition = new Position(0, 0);
    private int turn = 0;
    private int score = 0;

    /// <summary>
    /// Number of rows in the grid.
    /// </summary>
    public int Rows => grid.GetLength(0);

    /// <summary>
    /// Number of columns in the grid.
    /// </summary>
    public int Cols => grid.GetLength(1);

    /// <summary>
    /// Current score.
    /// </summary>
    public int Score => score;

    /// <summary>
    /// Current turn.
    /// </summary>
    public int Turn
    {
        get => turn;
        private set
        {
            turn = value;
        }
    }

    /// <summary>
    /// Number of turns until the game is over.
    /// </summary>
    public int MaxTurns => maxTurns;

    /// <summary>
    /// Current robot position.
    /// </summary>
    public Position RobotPosition => robotPosition;

    /// <summary>
    /// Target position for the last move action performed.
    /// </summary>
    public Position TargetPosition { get; private set; }

    /// <summary>
    /// Direction for the last move action performed.
    /// </summary>
    public Direction MovementDirection { get; private set; }

    /// <summary>
    /// Indicates whether the game has ended.
    /// </summary>
    public bool GameOver => turn >= maxTurns;

    /// <summary>
    /// Instantiates a new Trash Picker game.
    /// </summary>
    ///
    /// <param name="rows">
    /// Number of rows in the game grid.
    /// </param>
    /// <param name="cols">
    /// Number of columns in the game grid.
    /// </param>
    /// <param name="maxTurns">
    /// Number of turns until the game is over.
    /// </param>
    /// <param name="trashSpawnChance">
    /// Probability that a cell will have trash on it when the map is created.
    /// </param>
    public TrashPickerGame(int rows, int cols, int maxTurns,
        double trashSpawnChance)
    {
        rng = new Random();

        grid = new InternalCellState[rows, cols];

        // Place trash on cell based on defined chance
        for (int r = 0; r < grid.GetLength(0); r++)
        {
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                grid[r, c] = rng.NextDouble() < trashSpawnChance
                    ? InternalCellState.Trash : InternalCellState.Empty;
            }
        }

        TargetPosition = robotPosition;

        this.maxTurns = maxTurns;
    }

    /// <summary>
    /// Indicates what the state of the cell at <paramref name="pos" /> is.
    /// </summary>
    ///
    /// <param name="pos">
    /// The position to check.
    /// </param>
    ///
    /// <returns>
    /// The cell state.
    /// </returns>
    public CellState CellAt(Position pos)
    {
        pos = WrapPosition(pos);
        return (CellState)grid[pos.Row, pos.Col];
    }

    /// <summary>
    /// Tries to perform a move action towards <paramref name="dir" />.
    /// </summary>
    ///
    /// <param name="dir">
    /// The direction to move in.
    /// </param>
    ///
    /// <returns>
    /// A boolean indicating whether movement was successful.
    /// </returns>
    public bool MoveRobot(Direction dir)
    {
        if (GameOver)
            return false;

        turn++;

        MovementDirection = dir;

        // Convert direction to target position
        Position pos = robotPosition + dir switch
        {
            Direction.North => new Position(-1, 0),
            Direction.East => new Position(0, 1),
            Direction.South => new Position(1, 0),
            Direction.West => new Position(0, -1),
            _ => throw new ArgumentException("Invalid direction")
        };

        // If robot would hit a wall, wrap around position
        pos = WrapPosition(pos);

        TargetPosition = pos;
        robotPosition = TargetPosition;

        return true;
    }

    /// <summary>
    /// Skips the current turn.
    /// </summary>
    public void SkipTurn()
    {
        if (GameOver)
            return;

        turn++;
    }

    /// <summary>
    /// Tries to perform a collect trash action at the robot's position.
    /// </summary>
    ///
    /// <returns>
    /// A boolean indicating whether trash could be collected.
    /// </returns>
    public bool CollectTrash()
    {
        if (GameOver)
            return false;

        turn++;

        // If there isn't a cell at the robot's position, fail the action
        if (grid[robotPosition.Row, robotPosition.Col] != InternalCellState.Trash)
        {
            score -= 1;
            return false;
        }

        // Remove trash from cell if trash is picked up
        SetCell(robotPosition, InternalCellState.Empty);

        score += 10;

        return true;
    }

    private void SetCell(Position pos, InternalCellState state)
    {
        grid[pos.Row, pos.Col] = state;
    }

    private Position WrapPosition(Position pos)
    {
        if (pos.Row < 0)
        {
            return new Position(Rows - 1, pos.Col);
        }
        else if (pos.Row >= Rows)
        {
            return new Position(0, pos.Col);
        }
        else if (pos.Col < 0)
        {
            return new Position(pos.Row, Cols - 1);
        }
        else if (pos.Col >= Cols)
        {
            return new Position(pos.Row, 0);
        }

        return pos;
    }

    private bool IsPositionIllegal(Position p)
    {
        // Is position out of bounds?
        return p.Row < 0 || p.Row >= grid.GetLength(0) || p.Col < 0
            || p.Col >= grid.GetLength(1);
    }

    private bool IsCellVisible(Position pos)
    {
        // Von Neumann neighborhood
        IList<Position> neighborhood = new List<Position>
        {
            robotPosition,
            robotPosition + new Position(-1, 0),
            robotPosition + new Position(0, 1),
            robotPosition + new Position(1, 0),
            robotPosition + new Position(0, -1)
        };

        foreach (Position p in neighborhood)
        {
            if (pos == p) return true;
        }

        return false;
    }
}
