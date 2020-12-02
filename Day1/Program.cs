using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            var expenses = new List<int>();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Day1.input.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        expenses.Add(int.Parse(line));
                    }
                }
            }

            Part1(expenses);
            Console.WriteLine();
            Part2(expenses);
        }

        private static void Part1(List<int> expenses)
        {
            Console.WriteLine("PART 1");
            Console.WriteLine("---------------------------");

            var solutionFound = false;

            for (int i = 0; i < expenses.Count; i++)
            {
                if (solutionFound)
                {
                    break;
                }

                for (int j = 0; j < expenses.Count; j++)
                {
                    if (solutionFound)
                    {
                        break;
                    }

                    if (expenses[i] + expenses[j] == 2020)
                    {
                        solutionFound = true;

                        Console.WriteLine($"Found i = {i}: {expenses[i]} j = {j}: {expenses[j]}");

                        Console.WriteLine($"Multiplication result: {expenses[i] * expenses[j]}");
                    }
                }
            }
        }

        private static void Part2(List<int> expenses)
        {
            Console.WriteLine("PART 2");
            Console.WriteLine("---------------------------");

            var solutionFound = false;

            for (int i = 0; i < expenses.Count; i++)
            {
                if (solutionFound)
                {
                    break;
                }

                for (int j = 0; j < expenses.Count; j++)
                {
                    if (solutionFound)
                    {
                        break;
                    }

                    for (int k = 0; k < expenses.Count; k++)
                    {
                        if (solutionFound)
                        {
                            break;
                        }

                        if (expenses[i] + expenses[j] + expenses[k] == 2020)
                        {
                            solutionFound = true;

                            Console.WriteLine(
                                $"Found i = {i}: {expenses[i]} j = {j}: {expenses[j]} k = {k}: {expenses[k]}");

                            Console.WriteLine($"Multiplication result: {expenses[i] * expenses[j] * expenses[k]}");
                        }
                    }
                }
            }
        }
    }
}