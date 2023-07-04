using System;
using System.Collections.Generic;
using GA;

public class SplitBreeder<T>
{
    private Random rng;

    public SplitBreeder()
    {
        rng = new Random();
    }

    public Individual<T>[] Breed(Individual<T>[] population)
    {
        Individual<T>[] newPop = new Individual<T>[population.Length];
        int childCount = 0;

        while (childCount < newPop.Length)
        {
            // Choose 2 parents
            Individual<T> p1 = population[rng.Next(population.Length)];
            Individual<T> p2 = population[rng.Next(population.Length)];

            int splitPoint = rng.Next(population.Length);

            // Build child 1
            IList<T> genes = new List<T>();
            for (int i = 0; i < splitPoint; i++)
            {
                genes.Add(p1.Genes[i]);
            }
            for (int i = splitPoint; i < p1.Genes.Count; i++)
            {
                genes.Add(p2.Genes[i]);
            }
            Individual<T> c1 = new Individual<T>(genes);
            newPop[childCount++] = c1;

            // Build child 2
            genes = new List<T>();
            for (int i = 0; i < splitPoint; i++)
            {
                genes.Add(p2.Genes[i]);
            }
            for (int i = splitPoint; i < p1.Genes.Count; i++)
            {
                genes.Add(p1.Genes[i]);
            }
            Individual<T> c2 = new Individual<T>(genes);
            newPop[childCount++] = c2;
        }

        return newPop;
    }
}
