using System;

namespace GA
{
    public class GeneticAlgorithm<T>
    {
        private int maxGenerations;
        private int populationSize;
        private Individual<T>[] population;
        private Func<Individual<T>> generator;
        private Func<Individual<T>, float> evaluator;
        private Func<Individual<T>[], Individual<T>[]> breeder;
        private Func<Individual<T>[], Individual<T>[]> mutator;
        private float bestFitness;
        private Individual<T> bestIndividual;

        public GeneticAlgorithm(int populationSize, int maxGenerations,
            Func<Individual<T>> generator,
            Func<Individual<T>, float> evaluator,
            Func<Individual<T>[], Individual<T>[]> breeder,
            Func<Individual<T>[], Individual<T>[]> mutator)
        {
            this.populationSize = populationSize;
            this.maxGenerations = maxGenerations;
            this.generator = generator;
            this.evaluator = evaluator;
            this.breeder = breeder;
            this.mutator = mutator;

            population = new Individual<T>[populationSize];
            bestFitness = float.MinValue;

            for (int i = 0; i < populationSize; i++)
            {
                population[i] = generator.Invoke();
            }
        }

        public Individual<T> Run()
        {
            for (int g = 0; g < maxGenerations; g++)
            {
                // Evaluate fitness
                for (int i = 0; i < populationSize; i++)
                {
                    population[i].Fitness = evaluator.Invoke(population[i]);

                    if (population[i].Fitness.HasValue
                        && population[i].Fitness.Value > bestFitness)
                    {
                        bestIndividual = population[i];
                        bestFitness = population[i].Fitness.Value;
                    }
                }

                // Mating
                Individual<T>[] newPopulation = breeder.Invoke(population);

                // Mutation
                newPopulation = mutator.Invoke(newPopulation);
            }

            return bestIndividual;
        }
    }
}
