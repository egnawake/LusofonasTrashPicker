using System;
using System.Collections.Generic;

public class TrashPickerGame
{
    private readonly int maxTurns;
    private readonly InternalCellState[,] grid;
    private readonly Random rng;

    private Position robotPosition = new Position(0, 0);
    private int turn = 0;
    private int score = 0;

    public int Rows => grid.GetLength(0);
    public int Cols => grid.GetLength(1);

    public int Score => score;

    public int Turn
    {
        get => turn;
        private set
        {
            turn = value;
        }
    }
    public int MaxTurns => maxTurns;

    public Position RobotPosition => robotPosition;
    public Position TargetPosition { get; private set; }

    public bool GameOver => turn >= maxTurns;

    public TrashPickerGame(int rows, int cols, int maxTurns, double trashSpawnChance)
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

    public CellState CellAt(Position pos)
    {
        if (IsPositionIllegal(pos))
        {
            return CellState.Wall;
        }
        else if (IsCellVisible(pos))
        {
            return (CellState)grid[pos.Row, pos.Col];
        }
        else
        {
            return CellState.Hidden;
        }
    }

    public bool MoveRobot(Direction dir)
    {
        if (GameOver)
            return false;

        turn++;

        // Convert direction to target position
        TargetPosition = robotPosition + dir switch
        {
            Direction.North => new Position(-1, 0),
            Direction.East => new Position(0, 1),
            Direction.South => new Position(1, 0),
            Direction.West => new Position(0, -1),
            _ => throw new ArgumentException("Invalid direction")
        };

        // If robot would hit a wall, movement fails and score is reduced
        if (IsPositionIllegal(TargetPosition))
        {
            score -= 5;
            return false;
        }

        robotPosition = TargetPosition;

        return true;
    }

    public void SkipTurn()
    {
        if (GameOver)
            return;

        turn++;
    }

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
