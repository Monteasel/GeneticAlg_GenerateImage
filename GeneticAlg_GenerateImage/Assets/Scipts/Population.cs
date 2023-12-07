using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public Texture2D target;
    public int popMax;
    public float mutationRate;

    public DNA[] population;
    public List<DNA> matingPool;

    public float avgFitness;
    public float maxFitness;
    public int generations;

    public Population(Texture2D target, int popMax, float mutationRate)
    {
        this.target = target;
        this.popMax = popMax;
        this.mutationRate = mutationRate;

        population= new DNA[popMax];
        matingPool = new List<DNA>();

        for(int i = 0; i < popMax; i++)
        {
            population[i] = new DNA(target.width, target.height);
        }
    }

    public void calcFitness()
    {
        // Calculate fitness for each element in population

        int totalFitness = 0;
        for(int i = 0; i < population.Length; i++)
        {
            population[i].calcFitness(target);
            avgFitness += population[i].fitness;
        }
        avgFitness = (float)totalFitness/population.Length;
    }

    public void generate()
    {
        // Get max fitness
        for(int i = 0; i < population.Length; i++)
        {
            if (population[i].fitness > maxFitness)
            {
                maxFitness= population[i].fitness;
            }
        }

        // Generate new population
        DNA[] newPopulation = new DNA[popMax];
        for(int i = 0; i < population.Length; i++)
        {
            DNA parentA = acceptReject();
            DNA parentB = acceptReject();
            DNA child = parentA.crossover(parentB, target);
            child.mutate(mutationRate);
            newPopulation[i] = child;
        }

        population = newPopulation;
        generations++;
    }

    public DNA acceptReject()
    {
        int failSafe = 0;
        while (true)
        {
            int index = Mathf.FloorToInt(Random.Range(0, population.Length));
            DNA parent = population[index];
            float p = Random.Range(0, maxFitness);
            if (p < parent.fitness)
            {
                return parent;
            }
            failSafe++;
            if(failSafe > 10_000)
            {
                return null;
            }
        }
    }
}
