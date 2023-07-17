using System;
using System.Collections.Generic;

namespace GA
{
    public class GeneticAlgorithm<T>
    {
        private int maxGenerations;
        private int populationSize;
        private Individual<T>[] population;
        private Func<Individual<T>> generator;
        private Func<Individual<T>, float> evaluator;
        private Func<Individual<T>[], Individual<T>[]> crosser;
        private Func<Individual<T>[], Individual<T>[]> mutator;
        private float bestFitness;
        private Individual<T> bestIndividual;
        private IList<string> log;

        public GeneticAlgorithm(int populationSize, int maxGenerations,
            Func<Individual<T>> generator,
            Func<Individual<T>, float> evaluator,
            Func<Individual<T>[], Individual<T>[]> crosser,
            Func<Individual<T>[], Individual<T>[]> mutator,
            IList<string> log)
        {
            this.populationSize = populationSize;
            this.maxGenerations = maxGenerations;
            this.generator = generator;
            this.evaluator = evaluator;
            this.crosser = crosser;
            this.mutator = mutator;
            this.log = log;

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

                log.Add($"{bestFitness}");

                // Crossover
                Individual<T>[] newPopulation = crosser.Invoke(population);

                // Mutation
                newPopulation = mutator.Invoke(newPopulation);

                population = newPopulation;
            }

            return bestIndividual;
        }
    }
}
