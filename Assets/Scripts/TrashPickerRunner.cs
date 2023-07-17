using System;
using System.Collections.Generic;
using System.Linq;
using GA;

public class TrashPickerRunner
{
    private Random rng;
    private int runs;
    private int gridRows;
    private int gridColumns;
    private int maxTurns;
    private float trashProbability;

    public TrashPickerRunner(int runs, int gridRows, int gridColumns,
        int maxTurns, float trashProbability)
    {
        rng = new Random();
        this.runs = runs;
        this.gridRows = gridRows;
        this.gridColumns = gridColumns;
        this.maxTurns = maxTurns;
        this.trashProbability = trashProbability;
    }

    public float Evaluate(Individual<RobotAction> ind)
    {
        int scoreSum = 0;

        for (int i = 0; i < runs; i++)
        {
            scoreSum += Play(ind);
        }

        return scoreSum / runs;
    }

    private int Play(Individual<RobotAction> strategy)
    {
        TrashPickerGame game = new TrashPickerGame(gridRows, gridColumns,
            maxTurns, trashProbability);

        for (int i = 0; i < game.MaxTurns; i++)
        {
            int action = ToKey(game);

            if (strategy.Genes[action] == RobotAction.MoveNorth)
            {
                game.MoveRobot(Direction.North);
            }
            else if (strategy.Genes[action] == RobotAction.MoveEast)
            {
                game.MoveRobot(Direction.East);
            }
            else if (strategy.Genes[action] == RobotAction.MoveSouth)
            {
                game.MoveRobot(Direction.South);
            }
            else if (strategy.Genes[action] == RobotAction.MoveWest)
            {
                game.MoveRobot(Direction.West);
            }
            else if (strategy.Genes[action] == RobotAction.MoveRandom)
            {
                Direction d = (Direction)rng.Next(Enum.GetNames(typeof(Direction)).Length);
                game.MoveRobot(d);
            }
            else if (strategy.Genes[action] == RobotAction.SkipTurn)
            {
                game.SkipTurn();
            }
            else if (strategy.Genes[action] == RobotAction.CollectTrash)
            {
                game.CollectTrash();
            }
        }

        return game.Score;
    }

    private int ToKey(TrashPickerGame game)
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
