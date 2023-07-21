using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
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

        IList<string> evolutionLog = new List<string>();

        TrashPickerRunner runner = new TrashPickerRunner(gameRuns, gridRows,
            gridColumns, maxTurns, trashProbability);

        GeneticAlgorithm<RobotAction> ga = new GeneticAlgorithm<RobotAction>(
            populationSize, generations,
            new RandomEnumGenerator<RobotAction>(geneNumber).Generate,
            runner.Evaluate,
            new OnePointCrossover<RobotAction>().Cross,
            new RandomMutator<RobotAction>(mutationChance).Mutate,
            evolutionLog);

        DateTime start = DateTime.Now;
        Individual<RobotAction> result = ga.Run();
        TimeSpan time = DateTime.Now - start;

        Log(result, time, evolutionLog);
    }

    private void Log(Individual<RobotAction> ind, TimeSpan time,
        IEnumerable<string> log)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        string dirPath = Path.Combine(Application.persistentDataPath, $"ga_{date}");
        DirectoryInfo dir = Directory.CreateDirectory(dirPath);

        StringBuilder sb = new StringBuilder();
        sb.Append($"Population size: {populationSize}\n");
        sb.Append($"Generations: {generations}\n");
        sb.Append($"Mutation chance: {mutationChance}\n");
        sb.Append($"Game runs: {gameRuns}\n");
        sb.Append($"Grid size: {gridRows}x{gridColumns}\n");
        sb.Append($"Max turns: {maxTurns}\n");
        sb.Append($"Trash probability: {trashProbability}\n");
        sb.AppendFormat("Fitness: {0}\n", ind.Fitness.HasValue ? ind.Fitness.Value : "N/A");
        sb.Append($"Time: {time.ToString()}\n");
        sb.Append("\n");
        sb.Append(string.Join(',', log));
        sb.Append("\n");

        string filePath = Path.Combine(dir.FullName, "info.txt");
        File.WriteAllText(filePath, sb.ToString());

        filePath = Path.Combine(dir.FullName, "strategy.txt");
        File.WriteAllText(filePath, ind.ToString());
    }
}
