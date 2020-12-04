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

            var wire1 = GetVectors(lines[0]);
            var wire2 = GetVectors(lines[1]);

            Part1(wire1, wire2);

            Console.WriteLine();

            Part2(wire1, wire2);
        }

        private static void Part1(List<Vector> wire1, List<Vector> wire2)
        {
            ConsoleHelper.Part1();

            List<Point> intersectionPoints = new List<Point>();

            foreach (var wire1Vector in wire1)
            {
                intersectionPoints.AddRange(wire2.Select(v => v.Intersects(wire1Vector)).Where(x => x != null));
            }

            var shortestDistance = intersectionPoints.Select(x => x.X + x.Y).Min();

            Console.WriteLine($"Closest intersection distance: {shortestDistance}");
        }

        private static void Part2(List<Vector> wire1, List<Vector> wire2)
        {
            throw new NotImplementedException();
        }

        private static List<Vector> GetVectors(string line)
        {
            var result = new List<Vector>();

            var directions = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < directions.Length; i++)
            {
                var direction = directions[i];

                var vector = new Vector();
                if (i == 0)
                {
                    vector.From = new Point(0, 0);
                }
                else
                {
                    vector.From = (Point) result[i - 1].To.Clone();
                }

                vector.To = (Point) vector.From.Clone();

                var x = direction[0];
                var l = int.Parse(direction.Substring(1));

                switch (x)
                {
                    case 'L':
                        vector.To.X -= l;
                        break;
                    case 'R':
                        vector.To.X += l;
                        break;
                    case 'U':
                        vector.To.Y += l;
                        break;
                    case 'D':
                        vector.To.Y -= l;
                        break;
                }

                result.Add(vector);
            }

            return result;
        }

    }

    [DebuggerDisplay("{X},{Y}")]
    internal class Point : ICloneable
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public object Clone()
        {
            return new Point(X, Y);
        }

        protected bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    [DebuggerDisplay("{From},{To}")]
    internal class Vector
    {
        public Vector()
        {
        }

        public Vector(int x1, int y1, int x2, int y2)
        {
            From = new Point(x1, y1);
            To = new Point(x2, y2);
        }

        public Point From { get; set; }

        public Point To { get; set; }

        public bool IsHorizontal => From.Y == To.Y;

        public bool IsVertical => From.X == To.X;

        public Point Intersects(Vector vector)
        {
            if (IsHorizontal && vector.IsVertical)
            {
                if (vector.From.X >= MinX && vector.From.X <= MaxX &&
                    vector.MinY <= From.Y && vector.MaxY >= From.Y)
                {
                    return new Point(vector.From.X, From.Y);
                }
            }

            if (IsVertical && vector.IsHorizontal)
            {
                if (vector.From.Y >= MinY && vector.From.Y <= MaxY &&
                    vector.MinX <= From.X && vector.MaxX >= From.X)
                {
                    return new Point(From.X, vector.From.Y);
                }
            }

            return null;
        }

        private int MinX => Math.Min(From.X, To.X);

        private int MinY => Math.Min(From.Y, To.Y);

        private int MaxX => Math.Max(From.X, To.X);

        private int MaxY => Math.Max(From.Y, To.Y);
    }
}