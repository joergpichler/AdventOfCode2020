using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {
            const string testInput = "389125467";
            const string input = "643719258";

            var cups = testInput.Select(c => int.Parse(c.ToString())).ToArray();

            Part1(cups);
            
            Console.WriteLine();

            Part2(cups);
        }

        private static void Part1(int[] cups)
        {
            var game = new CupGame(cups);

            for (int i = 0; i < 100; i++)
            {
                if (game.IsDebug)
                {
                    Console.WriteLine($"-- move {i + 1} --");
                    Console.WriteLine(game.ToString());
                }

                game.Round();

                if (game.IsDebug)
                {
                    Console.ReadLine();
                }
            }

            ConsoleHelper.Part1();
            Console.WriteLine($"Result: {game.GetResultPt1()}");
        }

        private static void Part2(int[] cups)
        {
            var game = new CupGame(cups, true);

            for (int i = 0; i < 10000000; i++)
            {
                game.Round();
            }

            ConsoleHelper.Part1();
            Console.WriteLine($"Result: {game.GetResultPt2()}");
        }
    }

    class CupGame
    {
        private readonly List<int> _cups;
        private int _currentCup;
        private int _currentMove = 0;

        public CupGame(int[] cups, bool part2 = false)
        {
            _cups = cups.ToList();
            _currentCup = cups[0];

            if (part2)
            {
                var startingNumber = _cups.Max() + 1;

                for (int i = _cups.Count; i < 1000000; i++)
                {
                    _cups.Add(startingNumber);

                    startingNumber += 1;
                }
            }
        }

        public bool IsDebug { get; set; } = false;

        public void Round()
        {
            _currentMove += 1;
            
            // remove cups
            var removedCups = new int[3];
            var indicesToRemove = new int[3];
            var currentCupIndex = _cups.IndexOf(_currentCup);
            for (int i = 0; i < 3; i++)
            {
                var index = (currentCupIndex + i + 1) % _cups.Count;
                removedCups[i] = _cups[index];
                _cups[index] = -1;
                indicesToRemove[i] = index;
            }

            foreach (var indexToRemove in indicesToRemove.OrderByDescending(i => i))
            {
                _cups.RemoveAt(indexToRemove);
            }

            if (IsDebug)
            {
                Console.WriteLine($"pick up: {removedCups.Select(i => i.ToString()).Aggregate((a, b) => $"{a}, {b}")}");
            }
            
            // find destination cup
            var destinationCup = _currentCup - 1;
            while (!_cups.Contains(destinationCup))
            {
                destinationCup -= 1;
                if (destinationCup < _cups.Min())
                {
                    destinationCup = _cups.Max();
                }
            }

            if (IsDebug)
            {
                Console.WriteLine($"destination: {destinationCup}");
            }

            var destinationCupIndex = _cups.IndexOf(destinationCup);

            _cups.InsertRange(destinationCupIndex + 1, removedCups);

            _currentCup = _cups[(_cups.IndexOf(_currentCup) + 1) % _cups.Count];
        }

        public string GetResultPt1()
        {
            var sb = new StringBuilder();
            
            var indexOfOne = _cups.IndexOf(1);
            for (int i = 1; i < _cups.Count; i++)
            {
                sb.Append(_cups[(indexOfOne + i) % _cups.Count]);
            }

            return sb.ToString();
        }

        public string GetResultPt2()
        {
            var sb = new StringBuilder();

            var indexOfOne = _cups.IndexOf(1);
            long[] vals = new long[2];
            for (int i = 0; i < 2; i++)
            {
                vals[i] = (long) _cups[(indexOfOne + i) % _cups.Count];
            }

            return (vals[0] * vals[1]).ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder("cups: ");

            for (int i = 0; i < _cups.Count; i++)
            {
                var cup = _cups[i];
                if (_currentCup == cup)
                {
                    sb.Append($"({cup}) ");
                }
                else
                {
                    sb.Append($"{cup} ");
                }
            }
            
            return sb.ToString().Trim();
        }
    }
}
