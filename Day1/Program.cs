using System;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
             var moduleMasses = Assembly.GetExecutingAssembly()
                 .GetEmbeddedResourceLines("Day1.input.txt")
                 .Select(s => int.Parse(s)).ToArray();

            Part1(moduleMasses);

            Console.WriteLine();

            Part2(moduleMasses);
        }

        private static void Part1(int[] moduleMasses)
        {
            ConsoleHelper.Part1();

            Console.WriteLine($"Required fuel: {moduleMasses.Select(m => CalculateFuelRequirement(m, false)).Sum()}");
        }

        private static void Part2(int[] moduleMasses)
        {
            ConsoleHelper.Part2();

            Console.WriteLine($"Required fuel: {moduleMasses.Select(m => CalculateFuelRequirement(m, true)).Sum()}");
        }

        private static int CalculateFuelRequirement(int mass, bool recursive)
        {
            var requiredFuel = (int) Math.Floor(mass / 3.0) - 2;

            if (!recursive)
            {
                return requiredFuel;
            }

            if (requiredFuel <= 0)
            {
                return 0;
            }

            return requiredFuel + CalculateFuelRequirement(requiredFuel, true);
        }
    }
}