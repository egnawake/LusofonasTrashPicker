using System;
using System.IO;
using System.Text;
using Random = System.Random;
using UnityEngine;
using GA;

public class GAController : MonoBehaviour
{
    [SerializeField] private int populationSize = 200;
    [SerializeField] private int generations = 10;
    [SerializeField] private float mutationChance = 0.1f;
    [SerializeField] private int gameRuns = 100;
    [SerializeField] private int gridRows = 10;
    [SerializeField] private int gridColumns = 10;
    [SerializeField] private int maxTurns = 200;
    [SerializeField] private float trashProbability = 0.2f;

    // Total number of game situations
    private readonly int geneNumber = 243;

    private void Start()
    {
        Random rng = new Random();

        TrashPickerRunner runner = new TrashPickerRunner(gameRuns, gridRows,
            gridColumns, maxTurns, trashProbability);

        GeneticAlgorithm<RobotAction> ga = new GeneticAlgorithm<RobotAction>(
            populationSize, generations,
            new RandomEnumGenerator<RobotAction>(geneNumber).Generate,
            runner.Evaluate,
            new OnePointCrossover<RobotAction>().Cross,
            new RandomMutator<RobotAction>(mutationChance).Mutate);

        Individual<RobotAction> result = ga.Run();

        Debug.Log(result);
        Debug.Log(result.Fitness);

        SaveIndividual(result);

        LogResult(result.Fitness);
    }

    private void SaveIndividual(Individual<RobotAction> ind)
    {
        string path = Path.Combine(Application.persistentDataPath, "ga_out.txt");
        File.WriteAllText(path, ind.ToString());
    }

    private void LogResult(float? fitness)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"[{DateTime.Now}]\n");
        sb.Append($"Population size: {populationSize}\n");
        sb.Append($"Generations: {generations}\n");
        sb.Append($"Mutation chance: {mutationChance}\n");
        sb.Append($"Game runs: {gameRuns}\n");
        sb.Append($"Grid size: {gridRows}x{gridColumns}\n");
        sb.Append($"Max turns: {maxTurns}\n");
        sb.Append($"Trash probability: {trashProbability}\n");
        sb.AppendFormat("Fitness: {0}\n", fitness.HasValue ? fitness.Value : "N/A");
        sb.Append("\n");

        string path = Path.Combine(Application.persistentDataPath, "ga_log.txt");
        File.AppendAllText(path, sb.ToString());
    }
}
