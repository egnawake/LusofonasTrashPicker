using Random = System.Random;
using UnityEngine;
using GA;

public class GAController : MonoBehaviour
{
    private void Start()
    {
        Random rng = new Random();

        GeneticAlgorithm<RobotAction> ga = new GeneticAlgorithm<RobotAction>(
            100, 5, new RandomEnumGenerator<RobotAction>(243).Generate,
            (Individual<RobotAction> i) => (float)rng.NextDouble() * 3,
            (Individual<RobotAction>[] i) => i,
            (Individual<RobotAction>[] i) => i);

        Debug.Log(ga.Run());
    }
}
