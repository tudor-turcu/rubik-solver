using System;
using System.IO;
using Microsoft.Extensions.Logging;
using RubikCubeSolver.Kociemba.TwoPhase;

namespace rubikcubesolver
{
    class Program
    {
        static void Main(string[] args)
        {            
            ILogger logger = ApplicationLogging.CreateLogger<Program>();

            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the input file containing the cube configuration!");
                Environment.Exit(2);
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found!");
                Environment.Exit(2);
            }

            logger.LogDebug("Reading the input file..");

            string cubePermutation = File.ReadAllText(filePath);

            const int maxTime = 5;
            const int maxDepth = 21;

            string facelets = RemoveWhiteSpace(cubePermutation);

            string solution = Search.Solution(facelets, maxDepth, maxTime, useSeparator: true);

            Console.WriteLine($"Solution: \n\r {solution}");

            Console.ReadLine();
        }

        private static string RemoveWhiteSpace(string textPermutation)
        {
            return textPermutation.Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
        }
    }
}
