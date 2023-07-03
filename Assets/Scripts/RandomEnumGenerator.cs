using System;
using System.Collections.Generic;
using GA;

/// <summary>
/// GA generator strategy using enum types. Assumes enums have a "none" value
/// as the first member which is not a valid gene.
/// </summary>
public class RandomEnumGenerator<T> where T : Enum
{
    private Random rng;
    private int geneNumber;

    public RandomEnumGenerator(int geneNumber)
    {
        this.geneNumber = geneNumber;
        rng = new Random();
    }

    public Individual<T> Generate()
    {
        return new Individual<T>(RandomGenes());
    }

    public IEnumerable<T> RandomGenes()
    {
        int max = Enum.GetNames(typeof(T)).Length;

        for (int i = 0; i < geneNumber; i++)
        {
            T value = (T)Enum.ToObject(typeof(T), rng.Next(1, max));
            yield return value;
        }
    }
}
