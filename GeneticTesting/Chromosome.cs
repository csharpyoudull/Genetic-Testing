using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticTesting
{
    public class Chromosome:List<Dna>
    {
        /// <summary>
        /// Gets or sets the randomizer.
        /// </summary>
        /// <value>
        /// The randomizer.
        /// </value>
        private Random Randomizer { get; set; }

        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        /// <value>
        /// The fitness.
        /// </value>
        public double Fitness { get; set; }

        /// <summary>
        /// Gets the binary string.
        /// </summary>
        /// <value>
        /// The binary string.
        /// </value>
        public string BinaryString { get { return this.Aggregate(string.Empty, (current, item) => current += item.Binary); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chromosome"/> class.
        /// </summary>
        /// <param name="ranzomizer">The ranzomizer.</param>
        public Chromosome(Random ranzomizer) 
        {
            Randomizer = ranzomizer;
        }

        /// <summary>
        /// Mutates this instance.
        /// </summary>
        public void Mutate() 
        {
            var bString = BinaryString;
            var mutationIndex = Randomizer.Next(0, bString.Length);
            if (mutationIndex == bString.Length)
                return;

            var first = bString.Substring(0, mutationIndex);
            var second = bString.Substring(mutationIndex, bString.Length - mutationIndex);
            var mutated = string.Empty;
            for (var i = 0; i < second.Length; i++) 
            {
                var randomValue = Randomizer.Next(0, 1);
                mutated += randomValue.Equals(0) ? second[i].ToString() : Randomizer.Next(0, 1).ToString() ;
            }

            var newDna = Chromosome.FromBinaryString(first + mutated, Randomizer);
            this.Clear();
            this.AddRange(newDna);
        }

        /// <summary>
        /// Froms the binary string.
        /// </summary>
        /// <param name="binaryString">The binary string.</param>
        /// <param name="randomizer">The randomizer.</param>
        /// <returns></returns>
        public static Chromosome FromBinaryString(string binaryString, Random randomizer) 
        {
            var chromosome = new Chromosome(randomizer);
            while (binaryString.Any()) 
            {
                chromosome.Add(new Dna { Binary = binaryString.Substring(0,8) });
                binaryString = binaryString.Remove(0, 8);
            }
            return chromosome;
        }

        /// <summary>
        /// Crossovers the specified c1.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns></returns>
        public static Chromosome Crossover(Chromosome c1, Chromosome c2) 
        {
            var c1Dna = c1.BinaryString;
            var c2Dna = c2.BinaryString;
            var crossoverIndex = c1.Randomizer.Next(0, c1Dna.Length);
            if (crossoverIndex.Equals(c1Dna.Length))
                return c1;

            var newDna = c1Dna.Substring(0, crossoverIndex);
            newDna += c2Dna.Substring(crossoverIndex, c2Dna.Length - crossoverIndex);
            return Chromosome.FromBinaryString(newDna, c1.Randomizer);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Aggregate(string.Empty, (current, item) => current += item.Value);
        }
    }
}
