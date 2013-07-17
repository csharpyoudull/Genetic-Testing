using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var characterSet = "abcdefghijklmnopqrstuvwxyz";
            characterSet += characterSet.ToUpper();
            characterSet += "!.?, 0123456789";
            var ga = new GeneticAlgorithm("Hello world, I am Steve", 1500, 10000, .7, .3, characterSet);
            Console.ReadLine();
        }
    }
}
