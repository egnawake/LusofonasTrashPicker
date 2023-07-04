using System;
using GA;

public class RandomMutator<T>
{
    private Random rng;
    private double mutateProb;

    public RandomMutator(double mutateProb)
    {
        rng = new Random();
        this.mutateProb = mutateProb;
    }

    public Individual<T>[] Mutate(Individual<T>[] population)
    {
        Individual<T>[] newPop = new Individual<T>[population.Length];

        for (int i = 0; i < population.Length; i++)
        {
            Individual<T> ind = new Individual<T>(population[i].Genes);

            for (int j = 0; j < ind.Genes.Count; j++)
            {
                double roll = rng.NextDouble();
                if (roll < mutateProb)
                {
                    int geneValue = rng.Next(Enum.GetNames(typeof(T)).Length);
                    T gene = (T)Enum.ToObject(typeof(T), geneValue);

                    ind.Genes[j] = gene;
                }
                newPop[i] = ind;
            }
        }

        return newPop;
    }
}