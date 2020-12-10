using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var adapters = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day10.input.txt")
                .Select(x => int.Parse(x)).ToArray();

            Part1(adapters.ToList());
        }

        private static void Part1(IList<int> adapters)
        {
            var orderedAdapters = OrderAdapters(adapters);

            var countDiff1 = 0;
            var countDiff3 = 0;

            for (var i = 0; i < orderedAdapters.Count - 1; i++)
            {
                if (orderedAdapters[i] + 1 == orderedAdapters[i + 1])
                {
                    countDiff1 += 1;
                }
                else if (orderedAdapters[i] + 3 == orderedAdapters[i + 1])
                {
                    countDiff3 += 1;
                }
            }

            Console.WriteLine($"No of 1-joltage differences: {countDiff1}");
            Console.WriteLine($"No of 3-joltage differences: {countDiff3}");
            Console.WriteLine($"Multiplication result: {countDiff1 * countDiff3}");
        }

        private static IList<int> OrderAdapters(IList<int> adapters)
        {
            var orderedAdapters = new List<int>(adapters.Count + 1);

            var currentRating = 0;
            orderedAdapters.Add(currentRating); // outlet

            while (adapters.Count > 0)
            {
                var adapterFound = false;

                for (var i = 1; i <= 3; i++)
                {
                    var rating = currentRating + i;

                    if (adapters.Contains(rating))
                    {
                        adapters.Remove(rating);
                        orderedAdapters.Add(rating);
                        currentRating = rating;
                        adapterFound = true;
                        break;
                    }
                }

                if (!adapterFound)
                {
                    throw new InvalidOperationException();
                }
            }

            orderedAdapters.Add(orderedAdapters[^1] + 3); // device

            return orderedAdapters;
        }
    }
}
