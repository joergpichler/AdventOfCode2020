using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lib;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = Assembly.GetExecutingAssembly()
                .GetEmbeddedResourceLines("Day17.input.txt");

            Part1(lines);
            
            Console.WriteLine();

            Part2(lines);
        }

        private static void Part1(IEnumerable<string> lines)
        {
            var space4D = Space4D.Parse(lines);
            var activeCubes = Calc(space4D, false, false);
            
            ConsoleHelper.Part1();
            Console.WriteLine($"Active cubes: {activeCubes}");
        }

        private static void Part2(IEnumerable<string> lines)
        {
            var space4D = Space4D.Parse(lines);
            var activeCubes = Calc(space4D, true, false);

            ConsoleHelper.Part2();
            Console.WriteLine($"Active cubes: {activeCubes}");
        }

        private static int Calc(Space4D space4D, bool is4D, bool debug = false)
        {
            for (var cycle = 1; cycle <= 6; cycle++)
            {
                if (debug && !is4D)
                {
                    Console.WriteLine($"Cycle {cycle}");
                    Console.ReadLine();

                    space4D.Print();
                }

                var dim = space4D.Dimensions4D;

                Parallel.For(is4D ? dim.MinW - 1 : 0, is4D ? dim.MaxW + 2 : 1, w =>
                {
                    for (var z = dim.MinZ - 1; z <= dim.MaxZ + 1; z++)
                    {
                        for (var x = dim.MinX - 1; x <= dim.MaxX + 1; x++)
                        {
                            for (var y = dim.MinY - 1; y <= dim.MaxY + 1; y++)
                            {
                                var isActive = space4D.IsActive(x, y, z, w);
                                var activeNeighbors = space4D.GetActiveNeighbors(x, y, z, w).Count();

                                if (isActive && !(activeNeighbors == 2 || activeNeighbors == 3))
                                {
                                    space4D.AddChange(x, y, z, w, false);
                                }
                                else if (!isActive && activeNeighbors == 3)
                                {
                                    space4D.AddChange(x, y, z, w, true);
                                }
                            }
                        }
                    }
                });

                space4D.ApplyChanges();
            }

            return space4D.ActiveCount;
        }
    }

    class Space4D
    {
        private readonly HashSet<Point4D> _activePoints;

        private readonly ConcurrentBag<Point4DChange> _changes = new();

        public Space4D(IList<Point4D> activePoints)
        {
            _activePoints = activePoints.ToHashSet();
        }

        public Dimensions4D Dimensions4D
        {
            get
            {
                int minX = int.MaxValue;
                int maxX = int.MinValue;
                int minY = int.MaxValue;
                int maxY = int.MinValue;
                int minZ = int.MaxValue;
                int maxZ = int.MinValue;
                int minW = int.MaxValue;
                int maxW = int.MinValue;

                foreach (var point4D in _activePoints)
                {
                    minX = Math.Min(minX, point4D.X);
                    maxX = Math.Max(maxX, point4D.X);
                    minY = Math.Min(minY, point4D.Y);
                    maxY = Math.Max(maxY, point4D.Y);
                    minZ = Math.Min(minZ, point4D.Z);
                    maxZ = Math.Max(maxZ, point4D.Z);
                    minW = Math.Min(minW, point4D.W);
                    maxW = Math.Max(maxW, point4D.W);
                }

                return new(minX, maxX, minY, maxY, minZ, maxZ, minW, maxW);
            }
        }

        public int ActiveCount => _activePoints.Count;

        public void Print()
        {
            var dim = Dimensions4D;

            // Only print 3D space
            if (dim.MinW != 0 || dim.MaxW != 0)
            {
                return;
            }

            foreach (var zGroup in _activePoints.GroupBy(p => p.Z).OrderBy(g => g.Key))
            {
                var z = zGroup.Key;

                Console.WriteLine($"z={z}");

                var activePointsInGroup = zGroup.ToList();

                for (var y = dim.MinY; y <= dim.MaxY; y++)
                {
                    for (var x = dim.MinX; x <= dim.MaxX; x++)
                    {
                        Console.Write(activePointsInGroup.Any(p => p.X == x && p.Y == y && p.Z == z) ? '#' : '.');
                    }

                    Console.WriteLine();
                }
            }
        }

        public bool IsActive(int x, int y, int z, int w)
        {
            return _activePoints.Contains(new Point4D(x, y, z, w));
        }

        public IEnumerable<Point4D> GetActiveNeighbors(int x, int y, int z, int w)
        {
            for (var dx = x - 1; dx <= x + 1; dx++)
            {
                for (var dy = y - 1; dy <= y + 1; dy++)
                {
                    for (var dz = z - 1; dz <= z + 1; dz++)
                    {
                        for (var dw = w - 1; dw <= w + 1; dw++)
                        {
                            if (dx == x && dy == y && dz == z && dw == w)
                            {
                                continue;
                            }

                            var point = new Point4D(dx, dy, dz, dw);

                            if (_activePoints.Contains(point))
                            {
                                yield return point;
                            }
                        }
                    }
                }
            }
        }

        public void AddChange(int x, int y, int z, int w, bool isActive)
        {
            _changes.Add(new Point4DChange(new Point4D(x, y, z, w), isActive));
        }

        public void ApplyChanges()
        {
            foreach (var change in _changes)
            {
                if (!change.IsActive)
                {
                    if (!_activePoints.Remove(change.Point4D))
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    if (_activePoints.Contains(change.Point4D))
                    {
                        throw new InvalidOperationException();
                    }

                    _activePoints.Add(change.Point4D);
                }
            }

            _changes.Clear();
        }

        public static Space4D Parse(IEnumerable<string> lines)
        {
            var activePoints = new List<Point4D>();

            var charArrayLines = lines.Select(x => x.ToCharArray()).ToArray();

            for (var y = 0; y < charArrayLines.Length; y++)
            {
                var line = charArrayLines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        activePoints.Add(new Point4D(x, y, 0, 0));
                    }
                }
            }

            var space = new Space4D(activePoints);

            return space;
        }
    }

    [DebuggerDisplay("{X},{Y},{Z},{W}")]
    readonly struct Point4D
    {
        public Point4D(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public int W { get; }

        public static bool operator ==(Point4D p1, Point4D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point4D p1, Point4D p2)
        {
            return !p1.Equals(p2);
        }

        public bool Equals(Point4D other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object obj)
        {
            return obj is Point4D other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }
    }

    class Dimensions4D
    {
        public Dimensions4D(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, int minW, int maxW)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
            MinW = minW;
            MaxW = maxW;
        }

        public int MinX { get; }

        public int MaxX { get; }

        public int MinY { get; }

        public int MaxY { get; }

        public int MinZ { get; }

        public int MaxZ { get; }

        public int MinW { get; }

        public int MaxW { get; }
    }

    class Point4DChange
    {
        public Point4DChange(Point4D point4D, bool isActive)
        {
            Point4D = point4D;
            IsActive = isActive;
        }

        public Point4D Point4D { get; }

        public bool IsActive { get; }
    }
}