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

    private int geneNumber = 243;

    private void Start()
    {
        Random rng = new Random();

        TrashPickerRunner runner = new TrashPickerRunner(gameRuns, gridRows,
            gridColumns, maxTurns, trashProbability);

        GeneticAlgorithm<RobotAction> ga = new GeneticAlgorithm<RobotAction>(
            populationSize, generations,
            new RandomEnumGenerator<RobotAction>(geneNumber).Generate,
            runner.Evaluate,
            new SplitBreeder<RobotAction>().Breed,
            new RandomMutator<RobotAction>(mutationChance).Mutate);

        Individual<RobotAction> result = ga.Run();

        Debug.Log(result);
        Debug.Log(result.Fitness);
    }
}
