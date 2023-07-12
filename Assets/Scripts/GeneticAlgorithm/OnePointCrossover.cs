using System;
using System.Collections.Generic;
using GA;

public class OnePointCrossover<T>
{
    private Random rng;

    public OnePointCrossover()
    {
        rng = new Random();
    }

    public Individual<T>[] Cross(Individual<T>[] population)
    {
        Individual<T>[] newPop = new Individual<T>[population.Length];
        int childCount = 0;

        while (childCount < newPop.Length)
        {
            // Choose 2 parents
            Individual<T> p1 = SelectParent(population);
            Individual<T> p2 = SelectParent(population);

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

    private Individual<T> SelectParent(Individual<T>[] population)
    {
        Individual<T> ind1 = population[rng.Next(population.Length)];
        Individual<T> ind2 = population[rng.Next(population.Length)];

        if (ind1.Fitness > ind2.Fitness) return ind1;
        else return ind2;
    }
}
