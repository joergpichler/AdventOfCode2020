using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var memory = Assembly.GetExecutingAssembly()
                .GetEmbeddedResourceLines("Day2.input.txt").Single()
                .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();

            Part1(memory);

            Console.WriteLine();

            Part2(memory);
        }

        private static void Part1(int[] memory)
        {
            ConsoleHelper.Part1();

            int[] copy = new int[memory.Length];
            Array.Copy(memory, copy, memory.Length);

            copy[1] = 12;
            copy[2] = 2;

            RunMachine(copy);

            Console.WriteLine($"Value at position 0: {copy[0]}");
        }


        private static void Part2(int[] memory)
        {
            ConsoleHelper.Part2();

            for (int noun = 0; noun < 100; noun++)
            {
                int[] copy = new int[memory.Length];

                for (var verb = 0; verb < 100; verb++)
                {
                    Array.Copy(memory, copy, memory.Length);

                    copy[1] = noun;
                    copy[2] = verb;

                    RunMachine(copy);

                    if (copy[0] == 19690720)
                    {
                        Console.WriteLine($"Solution found! Noun: {noun} Verb: {verb} Result: {100 * noun + verb}");
                        break;
                    }
                }

                if (copy[0] == 19690720)
                {
                    break;
                }
            }
        }

        private static void RunMachine(int[] memory)
        {
            int ptr = 0;

            while (memory[ptr] != 99) // stop code
            {
                int operand1 = memory[memory[ptr + 1]];
                int operand2 = memory[memory[ptr + 2]];
                int result;

                switch (memory[ptr])
                {
                    case 1:
                        result = operand1 + operand2;
                        break;
                    case 2:
                        result = operand1 * operand2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                memory[memory[ptr + 3]] = result;

                ptr += 4;
            }
        }
    }
}