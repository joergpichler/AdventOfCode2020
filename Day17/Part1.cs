using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lib;

namespace Day17
{
    class Part1
    {
        public static void Run(IList<char[]> lines)
        {
            var activePoints = new List<Point3D>();

            for (var y = 0; y < lines.Count; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        activePoints.Add(new Point3D(x, y, 0));
                    }
                }
            }

            var space = new Space3D(activePoints);

            Calc(space, false);
        }

        private static void Calc(Space3D space3D, bool debug)
        {
            for (var cycle = 1; cycle <= 6; cycle++)
            {
                if (debug)
                {
                    Console.WriteLine($"Cycle {cycle}");
                    Console.ReadLine();

                    space3D.Print();
                }

                var dim = space3D.Dimensions3D;

                for (var z = dim.MinZ - 1; z <= dim.MaxZ + 1; z++)
                {
                    for (var x = dim.MinX - 1; x <= dim.MaxX + 1; x++)
                    {
                        for (var y = dim.MinY - 1; y <= dim.MaxY + 1; y++)
                        {
                            var isActive = space3D.IsActive(x, y, z);
                            var activeNeighbors = space3D.GetActiveNeighbors(x, y, z).Count();

                            if (isActive && !(activeNeighbors == 2 || activeNeighbors == 3))
                            {
                                space3D.AddChange(x, y, z, false);
                            }
                            else if (!isActive && activeNeighbors == 3)
                            {
                                space3D.AddChange(x, y, z, true);
                            }
                        }
                    }
                }

                space3D.ApplyChanges();
            }

            ConsoleHelper.Part1();
            Console.WriteLine($"Active cubes: {space3D.ActiveCount}");
        }
    }

    class Space3D
    {
        private readonly IList<Point3D> _activePoints;

        private readonly IList<Point3DChange> _changes = new List<Point3DChange>();

        public Space3D(IList<Point3D> activePoints)
        {
            _activePoints = activePoints;
        }

        public Dimensions3D Dimensions3D => Dimensions3D.Get(_activePoints);

        public int ActiveCount => _activePoints.Count;

        public void Print()
        {
            var dim = Dimensions3D;

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

        public bool IsActive(int x, int y, int z)
        {
            return _activePoints.Contains(new Point3D(x, y, z));
        }

        public IEnumerable<Point3D> GetActiveNeighbors(int x, int y, int z)
        {
            for (var dx = x - 1; dx <= x + 1; dx++)
            {
                for (var dy = y - 1; dy <= y + 1; dy++)
                {
                    for (var dz = z - 1; dz <= z + 1; dz++)
                    {
                        if (dx == x && dy == y && dz == z)
                        {
                            continue;
                        }

                        var point = new Point3D(dx, dy, dz);

                        if (_activePoints.Contains(point))
                        {
                            yield return point;
                        }
                    }
                }
            }
        }

        public void AddChange(int x, int y, int z, bool isActive)
        {
            _changes.Add(new Point3DChange(new Point3D(x, y, z), isActive));
        }

        public void ApplyChanges()
        {
            foreach (var change in _changes)
            {
                if (!change.IsActive)
                {
                    if (!_activePoints.Remove(change.Point3D))
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    if (_activePoints.Contains(change.Point3D))
                    {
                        throw new InvalidOperationException();
                    }

                    _activePoints.Add(change.Point3D);
                }
            }

            _changes.Clear();
        }
    }

    [DebuggerDisplay("{X},{Y},{Z}")]
    readonly struct Point3D
    {
        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public static bool operator ==(Point3D p1, Point3D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point3D p1, Point3D p2)
        {
            return !p1.Equals(p2);
        }

        public bool Equals(Point3D other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Point3D other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }

    class Dimensions3D
    {
        public Dimensions3D(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
        }

        public int MinX { get; }

        public int MaxX { get; }

        public int MinY { get; }

        public int MaxY { get; }

        public int MinZ { get; }

        public int MaxZ { get; }

        public static Dimensions3D Get(IList<Point3D> space)
        {
            return new(
                space.Min(p => p.X),
                space.Max(p => p.X),
                space.Min(p => p.Y),
                space.Max(p => p.Y),
                space.Min(p => p.Z),
                space.Max(p => p.Z)
            );
        }
    }

    class Point3DChange
    {
        public Point3DChange(Point3D point3D, bool isActive)
        {
            Point3D = point3D;
            IsActive = isActive;
        }

        public Point3D Point3D { get; }

        public bool IsActive { get; }
    }
}