using System;

namespace GA
{
    public class GeneticAlgorithm<T>
    {
        private Individual<T>[] population;
        private Func<Individual<T>> generator;

        public GeneticAlgorithm(int populationSize, Func<Individual<T>> generator)
        {
            population = new Individual<T>[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                population[i] = generator.Invoke();
            }
        }

        public Individual<T> Run()
        {
            return null;
        }
    }
}
