using System;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day9.input.txt")
                .Select(x => long.Parse(x)).ToArray();

            var preambleLength = 25;

            Part1(numbers, preambleLength);

            Console.WriteLine();

            Part2(numbers, preambleLength);
        }

        private static void Part1(long[] numbers, int preambleLength)
        {
            ConsoleHelper.Part1();

            if (!TryValidate(numbers, preambleLength, out var invalidNumber))
            {
                Console.WriteLine($"{invalidNumber} cannot be produced by summing two of the {preambleLength} previous numbers");
            }
        }

        private static void Part2(long[] numbers, in int preambleLength)
        {
            if (TryValidate(numbers, preambleLength, out var invalidNumber))
            {
                throw new InvalidOperationException();
            }

            if (TryGetRangeThatSumsTo(numbers, invalidNumber, out int startIndex, out int endIndex))
            {
                var foundNumbers = numbers.Skip(startIndex).Take(endIndex - startIndex + 1).ToArray();
                var minNumber = foundNumbers.Min();
                var maxNumber = foundNumbers.Max();

                Console.WriteLine($"The encryption weakness is {minNumber + maxNumber}");
            }
        }

        private static bool TryGetRangeThatSumsTo(long[] numbers, long targetSum, out int startIndex, out int endIndex)
        {
            for (startIndex = 0; startIndex < numbers.Length - 2; startIndex++)
            {
                long sum = 0;
                int i = startIndex;

                while (sum < targetSum)
                {
                    sum += numbers[i++];
                }

                if (sum == targetSum)
                {
                    endIndex = i - 1;
                    return true;
                }
            }

            startIndex = 0;
            endIndex = 0;

            return false;
        }

        private static bool TryValidate(long[] numbers, int preambleLength, out long invalidNumber)
        {
            if (numbers.Length < preambleLength)
            {
                throw new InvalidOperationException();
            }

            for (int i = preambleLength; i < numbers.Length; i++)
            {
                if (!CanTwoEntriesSumTo(numbers, i - preambleLength, i - 1, numbers[i]))
                {
                    invalidNumber = numbers[i];
                    
                    return false;
                }
            }

            invalidNumber = default;
            return true;
        }

        private static bool CanTwoEntriesSumTo(long[] numbers, int startIndex, int endIndex, long sum)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                for (int j = startIndex; j <= endIndex; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (numbers[i] + numbers[j] == sum)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
