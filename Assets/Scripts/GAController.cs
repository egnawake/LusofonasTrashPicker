using Random = System.Random;
using UnityEngine;
using GA;

public class GAController : MonoBehaviour
{
    private void Start()
    {
        Random rng = new Random();

        GeneticAlgorithm<RobotAction> ga = new GeneticAlgorithm<RobotAction>(
            100, 10, new RandomEnumGenerator<RobotAction>(243).Generate,
            new TrashPickerRunner().Evaluate,
            new SplitBreeder<RobotAction>().Breed,
            new RandomMutator<RobotAction>(0.1).Mutate);

        Individual<RobotAction> result = ga.Run();
        Debug.Log(result.Fitness);
    }
}
