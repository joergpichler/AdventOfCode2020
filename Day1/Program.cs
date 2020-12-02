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
             var expenses = Assembly.GetExecutingAssembly()
                 .GetEmbeddedResourceLines("Day1.input.txt")
                 .Select(s => int.Parse(s)).ToArray();

            Part1(expenses);

            Console.WriteLine();

            Part2(expenses);
        }

        private static void Part1(int[] expenses)
        {
            ConsoleHelper.Part1();

            var solutionFound = false;

            for (int i = 0; i < expenses.Length; i++)
            {
                if (solutionFound)
                {
                    break;
                }

                for (int j = 0; j < expenses.Length; j++)
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

        private static void Part2(int[] expenses)
        {
            ConsoleHelper.Part2();

            var solutionFound = false;

            for (int i = 0; i < expenses.Length; i++)
            {
                if (solutionFound)
                {
                    break;
                }

                for (int j = 0; j < expenses.Length; j++)
                {
                    if (solutionFound)
                    {
                        break;
                    }

                    for (int k = 0; k < expenses.Length; k++)
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