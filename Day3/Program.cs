using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day3.input.txt").ToList();

            var map = TreeMap.Parse(lines);

            Part1(map);

            Console.WriteLine();

            Part2(map);
        }

        private static void Part1(TreeMap treeMap)
        {
            ConsoleHelper.Part1();

            Console.WriteLine($"{treeMap.GetNoOfEncounteredTrees(3, 1)} trees found");
        }

        private static void Part2(TreeMap treeMap)
        {
            ConsoleHelper.Part2();

            var result = treeMap.GetNoOfEncounteredTrees(1, 1) * treeMap.GetNoOfEncounteredTrees(3, 1) *
                         treeMap.GetNoOfEncounteredTrees(5, 1) * treeMap.GetNoOfEncounteredTrees(7, 1) * treeMap.GetNoOfEncounteredTrees(1, 2);

            Console.WriteLine($"Multiplication result is {result}");
        }
    }

    [DebuggerDisplay("{X},{Y}")]
    internal class Point
    {
        public int X { get; set; }

        public int Y { get; set; }

        public void Add(int x, int y)
        {
            X += x;
            Y += y;
        }
    }

    internal class TreeMap
    {
        private readonly char[,] _map;

        private TreeMap(char[,] map)
        {
            _map = map;
        }

        public bool Contains(Point point)
        {
            return point.Y < _map.GetLength(1);
        }

        public char Get(int x, int y)
        {
            return _map[x % _map.GetLength(0), y];
        }

        public static TreeMap Parse(List<string> lines)
        {
            char[,] map = new char[lines[0].Length, lines.Count];

            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    map[x, y] = lines[y][x];
                }
            }

            return new TreeMap(map);
        }
    }

    internal static class TreeMapExtensions
    {
        public static int GetNoOfEncounteredTrees(this TreeMap treeMap, int slopeX, int slopeY)
        {
            var coords = new Point { X = 0, Y = 0 };
            coords.Add(slopeX, slopeY);

            int treeCtr = 0;

            while (treeMap.Contains(coords))
            {
                if (treeMap.Get(coords.X, coords.Y) == '#')
                {
                    treeCtr += 1;
                }

                coords.Add(slopeX, slopeY);
            }

            return treeCtr;
        }
    }
}