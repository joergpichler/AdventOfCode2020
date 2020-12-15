using System;
using System.Collections.Generic;
using System.Linq;
using Lib;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            var testInput = "0,3,6";
            var input = "5,1,9,18,13,8,0";

            var numbers = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x))
                .ToList();

            Part1(numbers);
            
            Console.WriteLine();
            
            Part2(numbers);
        }

        private static void Part1(List<int> numbers)
        {
            var game = new NumberGame();
            var lastNumber = game.Play(numbers, 2020);
            
            ConsoleHelper.Part1();
            Console.WriteLine($"Last number: {lastNumber}");
        }

        private static void Part2(List<int> numbers)
        {
            var game = new NumberGame();
            var lastNumber = game.Play(numbers, 30000000);

            ConsoleHelper.Part2();
            Console.WriteLine($"Last number: {lastNumber}");
        }
    }

    class NumberGame
    {
        private readonly Dictionary<int, NumberMemory> _numberHistory = new();

        private int _lastNumber;

        public int Play(IList<int> startingNumbers, int turns)
        {
            _numberHistory.Clear();

            for (int i = 0; i < turns; i++)
            {
                int number;

                if (startingNumbers.Count > i)
                {
                    number = startingNumbers[i];
                }
                else
                {
                    var numberMemory = _numberHistory[_lastNumber];
                    if (numberMemory.PreviousTurn == -1)
                    {
                        number = 0;
                    }
                    else
                    {
                        number = numberMemory.LastTurn - numberMemory.PreviousTurn;
                    }
                }

                _lastNumber = number;

                if (!_numberHistory.TryGetValue(number, out var memory))
                {
                    memory = new NumberMemory();
                    _numberHistory.Add(number, memory);
                }

                memory.Increment(i);
            }

            return _lastNumber;
        }
    }

    class NumberMemory
    {
        public int PreviousTurn { get; private set; } = -1;

        public int LastTurn { get; private set; } = -1;

        public int Count { get; private set; }

        public void Increment(int turn)
        {
            if (LastTurn != -1)
            {
                PreviousTurn = LastTurn;
            }

            LastTurn = turn;

            Count += 1;
        }
    }
}