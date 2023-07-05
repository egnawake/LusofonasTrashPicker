using System;
using System.Collections.Generic;
using System.Linq;
using GA;

public class TrashPickerRunner
{
    private Random rng;

    public TrashPickerRunner()
    {
        rng = new Random();
    }

    public float Evaluate(Individual<RobotAction> ind)
    {
        TrashPickerGame game = new TrashPickerGame(100, 100, 20, 0.3f);

        for (int i = 0; i < game.MaxTurns; i++)
        {
            int action = ToKey(game);

            if (ind.Genes[action] == RobotAction.MoveNorth)
            {
                game.MoveRobot(Direction.North);
            }
            else if (ind.Genes[action] == RobotAction.MoveEast)
            {
                game.MoveRobot(Direction.East);
            }
            else if (ind.Genes[action] == RobotAction.MoveSouth)
            {
                game.MoveRobot(Direction.South);
            }
            else if (ind.Genes[action] == RobotAction.MoveWest)
            {
                game.MoveRobot(Direction.West);
            }
            else if (ind.Genes[action] == RobotAction.MoveRandom)
            {
                Direction d = (Direction)rng.Next(Enum.GetNames(typeof(Direction)).Length);
                game.MoveRobot(d);
            }
            else if (ind.Genes[action] == RobotAction.SkipTurn)
            {
                game.SkipTurn();
            }
            else if (ind.Genes[action] == RobotAction.CollectTrash)
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
