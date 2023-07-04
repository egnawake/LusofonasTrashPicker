using Random = System.Random;
using UnityEngine;
using GA;

public class GAController : MonoBehaviour
{
    private void Start()
    {
        Random rng = new Random();

        GeneticAlgorithm<RobotAction> ga = new GeneticAlgorithm<RobotAction>(
            100, 20, new RandomEnumGenerator<RobotAction>(243).Generate,
            (Individual<RobotAction> i) => (float)rng.NextDouble() * 3,
            new SplitBreeder<RobotAction>().Breed,
            new RandomMutator<RobotAction>(0.1).Mutate);

        Debug.Log(ga.Run());
    }
}
