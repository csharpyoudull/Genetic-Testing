using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticTesting
{
    public class GeneticAlgorithm
    {
        private int PopulationSize { get; set; }
        private string KnownCharacters { get; set; }
        private List<Chromosome> Population { get; set; }
        private string FitnessMeasure { get; set; }
        private Random Randomizer{get;set;}
        private double CrossoverRate{get;set;}
        private double MutationRate{get;set;}
        private int CurrentEpoc { get; set; }
        private bool ProblemSolved { get; set; }
        private Chromosome Winner { get; set; }
        private double LastMostFit { get; set; }
        private double LastAvgFit { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgorithm"/> class.
        /// </summary>
        /// <param name="fitnessMeasure">The fitness measure.</param>
        /// <param name="populationSize">Size of the population.</param>
        /// <param name="maxGenerations">The max generations.</param>
        /// <param name="crossoverRate">The crossover rate.</param>
        /// <param name="mutationRate">The mutation rate.</param>
        /// <param name="characterSet">The character set.</param>
        public GeneticAlgorithm(string fitnessMeasure, int populationSize, int maxGenerations, double crossoverRate, double mutationRate, string characterSet) 
        {
            LastMostFit = 0;
            LastAvgFit = 0;
            PopulationSize = populationSize;
            FitnessMeasure = fitnessMeasure;
            KnownCharacters = characterSet;
            Randomizer = new Random();
            CrossoverRate = crossoverRate;
            MutationRate = mutationRate;
            CurrentEpoc = 1;
            GeneratePopulation(populationSize);
            var timer = Stopwatch.StartNew();
            PrintReport();
            while (CurrentEpoc < maxGenerations && !ProblemSolved) 
            {
                RunEpoc();
                PrintReport();
                CurrentEpoc++;                
            }
            timer.Stop();

            var message = new StringBuilder();
            message.AppendLine("Completed");
            if (ProblemSolved)
            {
                message.AppendLine(string.Format("Problem solved, Solution text: {0}", Winner.ToString()));
                message.AppendLine(string.Format("Generations required: {0}", CurrentEpoc));
            }
            else
            { 
                var mostFit = Population.Max(p => p.Fitness);
                var chromosome = Population.FirstOrDefault(p => p.Fitness.Equals(mostFit));
                message.AppendLine(string.Format("Failed to solve the problem in {0} generations", maxGenerations));
                message.AppendLine(string.Format("Most accurate solution is {0}% fit, Solution text: {1}", mostFit, chromosome.ToString()));
            }
            message.AppendLine(string.Format("Execution time: {0}",timer.Elapsed.ToString()));
            Console.WriteLine(message);
        }

        /// <summary>
        /// Generates the population.
        /// </summary>
        /// <param name="populationSize">Size of the population.</param>
        private void GeneratePopulation(int populationSize) 
        {
            Population = new List<Chromosome>();
            for (var i = 0; i < populationSize; i++) 
                Population.Add(GenerateRandomChromosome(FitnessMeasure.Length));
        }

        /// <summary>
        /// Generates the random chromosome.
        /// </summary>
        /// <param name="dnaLength">Length of the dna.</param>
        /// <returns></returns>
        private Chromosome GenerateRandomChromosome(int dnaLength) 
        {
            var maxSelectionLength = KnownCharacters.Length;
            var chromosome = new Chromosome(Randomizer);
            for (var i = 0; i < dnaLength; i++) {
                chromosome.Add(new Dna { Value = KnownCharacters[Randomizer.Next(0, maxSelectionLength)] });
            }
            return chromosome;
        }

        /// <summary>
        /// Runs the epoc.
        /// </summary>
        private void RunEpoc() 
        {
            var newPopulationMembers = new List<Chromosome>();
            var failedGenetics = new List<Chromosome>();

            foreach (var chromosome in Population)
            {
                if (Randomizer.NextDouble() <= CrossoverRate) 
                {
                    var crossoverTarget = Population[Randomizer.Next(0, Population.Count)];
                    while (crossoverTarget.Equals(chromosome)) 
                    {
                        crossoverTarget = Population[Randomizer.Next(0, Population.Count)];
                    }

                    var crossoverProduct = Chromosome.Crossover(chromosome, crossoverTarget);
                    if (ValidateChromosome(crossoverProduct))
                    {
                        crossoverProduct.Fitness = ScoreFitness(crossoverProduct);
                        newPopulationMembers.Add(crossoverProduct);
                    }
                }

                if (Randomizer.NextDouble() <= MutationRate)
                {
                    chromosome.Mutate();
                    if (!ValidateChromosome(chromosome))
                        failedGenetics.Add(chromosome);
                }

                chromosome.Fitness = ScoreFitness(chromosome);
            }
            Population = Population.Where(p => !failedGenetics.Any(fg => fg.Equals(p))).ToList();
            Population.AddRange(newPopulationMembers);
            Population = Population.OrderByDescending(p => p.Fitness).Take(PopulationSize).ToList();

            Winner = Population.FirstOrDefault(c => c.Fitness >= 100);
            if (Winner != null)
                ProblemSolved = true;
        }

        /// <summary>
        /// Validates the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns></returns>
        public bool ValidateChromosome(Chromosome chromosome) 
        {
            var result = chromosome.All(d => KnownCharacters.Any(c => c.Equals(d.Value)));
            return result;
        }

        /// <summary>
        /// Scores the fitness.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns></returns>
        private double ScoreFitness(Chromosome chromosome) 
        {
            var accurate = 0.0;
            var dna = chromosome.Aggregate(string.Empty, (current, item) => current += item.Value);
            for (var i = 0; i < FitnessMeasure.Length; i++) 
            {
                if (dna[i].Equals(FitnessMeasure[i]))
                    accurate++;
            }

            return (accurate / (double)FitnessMeasure.Length) * 100;
        }

        /// <summary>
        /// Prints the report.
        /// </summary>
        private void PrintReport() 
        {
            var mostFit = Population.Max(p => p.Fitness);
            var avgFit = Population.Average(p => p.Fitness);
            if (mostFit != LastMostFit || avgFit != LastAvgFit)
            {
                LastMostFit = mostFit;
                LastAvgFit = avgFit;
                var chromosome = Population.FirstOrDefault(p => p.Fitness.Equals(mostFit));
                
                Console.WriteLine(string.Format("Current epoc: {0}", CurrentEpoc));
                Console.WriteLine(string.Format("Most fit score: {0}", mostFit));
                Console.WriteLine(string.Format("Most fit result: {0}",chromosome.ToString()));
                Console.WriteLine(string.Format("Average fitness: {0}", avgFit));
            }
        }
    }
}
