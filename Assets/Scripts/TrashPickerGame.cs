using System;

public class TrashPickerGame
{
    private readonly int maxSteps;
    private readonly CellState[,] grid;
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

        grid = new CellState[rows, cols];
        for (int r = 0; r < grid.GetLength(0); r++)
        {
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                grid[r, c] = rng.NextDouble() < trashSpawnChance
                    ? CellState.Trash : CellState.Empty;
            }
        }

        this.maxSteps = maxSteps;
    }

    public CellState CellAt(Position pos)
    {
        return grid[pos.Row, pos.Col];
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

        if (targetPos.Row < 0 || targetPos.Row >= grid.GetLength(0)
            || targetPos.Col < 0 || targetPos.Col >= grid.GetLength(1))
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

        if (CellAt(robotPosition) != CellState.Trash)
        {
            score -= 1;
            return false;
        }

        SetCell(robotPosition, CellState.Empty);
        score += 10;
        return true;
    }

    private void SetCell(Position pos, CellState state)
    {
        grid[pos.Row, pos.Col] = state;
    }
}
