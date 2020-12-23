using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            string inputToParse = input;
            
            Part1(ParseInput(inputToParse));
            
            Console.WriteLine();

            Part2(ParseInput(inputToParse));
        }

        private static Cup ParseInput(string input)
        {
            var cups = input.Select(c => new Cup(int.Parse(c.ToString()))).ToArray();

            for (var i = 0; i < cups.Length; i++)
            {
                if (i == cups.Length - 1)
                {
                    cups[i].Next = cups[0];
                }
                else
                {
                    cups[i].Next = cups[i + 1];
                }
            }

            return cups[0];
        }

        private static void Part1(Cup firstCup)
        {
            var game = new CupGame(firstCup);

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

        private static void Part2(Cup firstCup)
        {
            var game = new CupGame(firstCup, true);

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
        private Cup _currentCup;
        private int _currentMove = 0;
        private readonly Dictionary<int, Cup> _labelToCup = new Dictionary<int, Cup>();
        private readonly int _minLabel;
        private readonly int _maxLabel;

        public CupGame(Cup firstCup, bool part2 = false)
        {
            _currentCup = firstCup;

            var cup = firstCup;
            do
            {
                _labelToCup[cup.Label] = cup;
                cup = cup.Next;
            } while (cup != firstCup);

            if (part2)
            {
                Cup lastCup = null;
                var tmpCup = firstCup;
                while (lastCup == null)
                {
                    if (tmpCup.Next == firstCup)
                    {
                        lastCup = tmpCup;
                    }

                    tmpCup = tmpCup.Next;
                }

                var cupNo = _labelToCup.Keys.Max() + 1;
                for (int i = _labelToCup.Count; i < 1000000; i++)
                {
                    var newCup = new Cup(cupNo++);
                    _labelToCup[newCup.Label] = newCup;
                    lastCup.Next = newCup;
                    lastCup = newCup;
                }

                lastCup.Next = firstCup;
            }

            _minLabel = _labelToCup.Keys.Min();
            _maxLabel = _labelToCup.Keys.Max();
        }

        public bool IsDebug { get; set; } = false;

        public void Round()
        {
            _currentMove += 1;
            
            // remove cups
            var removedCups = new Cup[]
            {
                _currentCup.Next, _currentCup.Next.Next, _currentCup.Next.Next.Next
            };

            _currentCup.Next = removedCups[2].Next;
            removedCups[2].Next = removedCups[0];

            if (IsDebug)
            {
                Console.WriteLine($"pick up: {removedCups.Select(i => i.Label.ToString()).Aggregate((a, b) => $"{a}, {b}")}");
            }
            
            // find destination cup
            var destinationCupLabel = _currentCup.Label - 1;
            Cup destinationCup = null;
            
            while (destinationCup == null)
            {
                if (_labelToCup.TryGetValue(destinationCupLabel, out var tmpCup) && !removedCups.Contains(tmpCup))
                {
                    destinationCup = tmpCup;
                }
                
                destinationCupLabel -= 1;
                if (destinationCupLabel < _minLabel)
                {
                    destinationCupLabel = _maxLabel;
                }
            }

            if (IsDebug)
            {
                Console.WriteLine($"destination: {destinationCup.Label}");
            }

            // insert removed Cups
            var tmpCup2 = destinationCup.Next;
            destinationCup.Next = removedCups[0];
            removedCups[2].Next = tmpCup2;

            _currentCup = _currentCup.Next;
        }

        public string GetResultPt1()
        {
            var sb = new StringBuilder();

            var endCup = _labelToCup[1];
            var cup = endCup.Next;

            while (cup != endCup)
            {
                sb.Append(cup.Label);
                cup = cup.Next;
            }

            return sb.ToString();
        }

        public string GetResultPt2()
        {
            var cup = _labelToCup[1];

            return ((long) cup.Next.Label * (long) cup.Next.Next.Label).ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder("cups: ");

            var cup = _currentCup;
            do
            {
                if (cup == _currentCup)
                {
                    sb.Append($"({cup.Label}) ");
                }
                else
                {
                    sb.Append($"{cup.Label} ");
                }
                cup = cup.Next;
            } while (cup != _currentCup);
            
            return sb.ToString().Trim();
        }
    }

    [DebuggerDisplay("{Label}")]
    class Cup
    {
        public Cup(int label)
        {
            Label = label;
        }

        public int Label { get; }
        
        public Cup Next { get; set; }
    }
}
