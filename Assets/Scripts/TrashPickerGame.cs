using System;
using System.Collections.Generic;

public class TrashPickerGame
{
    private readonly int maxSteps;
    private readonly InternalCellState[,] grid;
    private readonly Random rng;

    private Position robotPosition = new Position(0, 0);
    private int step = 0;
    private int score = 0;

    // TODO: maybe make an enumerator for this class?
    public int Rows => grid.GetLength(0);
    public int Cols => grid.GetLength(1);

    public int Score => score;

    public int Step
    {
        get => step;
        private set
        {
            step = value;
        }
    }
    public int MaxSteps => maxSteps;

    public Position RobotPosition => robotPosition;

    public bool GameOver => step >= maxSteps;

    public TrashPickerGame(int rows, int cols, int maxSteps, double trashSpawnChance)
    {
        rng = new Random();

        grid = new InternalCellState[rows, cols];
        for (int r = 0; r < grid.GetLength(0); r++)
        {
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                grid[r, c] = rng.NextDouble() < trashSpawnChance
                    ? InternalCellState.Trash : InternalCellState.Empty;
            }
        }

        this.maxSteps = maxSteps;
    }

    public CellState CellAt(Position pos)
    {
        if (IsCellVisible(pos))
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

        step++;

        Position targetPos = robotPosition + dir switch
        {
            Direction.North => new Position(-1, 0),
            Direction.East => new Position(0, 1),
            Direction.South => new Position(1, 0),
            Direction.West => new Position(0, -1),
            _ => throw new ArgumentException("Invalid direction")
        };

        if (IsPositionIllegal(targetPos))
        {
            score -= 5;
            return false;
        }

        robotPosition = targetPos;
        return true;
    }

    public void SkipTurn()
    {
        if (GameOver)
            return;

        step++;
    }

    public bool CollectTrash()
    {
        if (GameOver)
            return false;

        step++;

        if (grid[robotPosition.Row, robotPosition.Col] != InternalCellState.Trash)
        {
            score -= 1;
            return false;
        }

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
        return p.Row < 0 || p.Row >= grid.GetLength(0) || p.Col < 0
            || p.Col >= grid.GetLength(1);
    }

    private bool IsCellVisible(Position pos)
    {
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
