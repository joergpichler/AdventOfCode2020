using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lib;

namespace Day17
{
    class Part2
    {
        public static void Run(IList<char[]> lines)
        {
            var activePoints = new List<Point4D>();

            for (var y = 0; y < lines.Count; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        activePoints.Add(new Point4D(x, y, 0, 0));
                    }
                }
            }

            var space = new Space4D(activePoints);

            Calc(space, false);
        }

        private static void Calc(Space4D space4D, bool debug)
        {
            for (var cycle = 1; cycle <= 6; cycle++)
            {
                if (debug)
                {
                    Console.WriteLine($"Cycle {cycle}");
                    Console.ReadLine();

                    //space3D.Print();
                }

                var dim = space4D.Dimensions4D;

                Parallel.For(dim.MinW - 1, dim.MaxW + 2, w =>
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

            ConsoleHelper.Part2();
            Console.WriteLine($"Active cubes: {space4D.ActiveCount}");
        }
    }

    class Space4D
    {
        private readonly IList<Point4D> _activePoints;

        private readonly ConcurrentBag<Point4DChange> _changes = new ();

        public Space4D(IList<Point4D> activePoints)
        {
            _activePoints = activePoints;
        }

        public Dimensions4D Dimensions4D => Dimensions4D.Get(_activePoints);

        public int ActiveCount => _activePoints.Count;

        //    public void Print()
        //    {
        //        var dim = Dimensions3D;

        //        foreach (var zGroup in _activePoints.GroupBy(p => p.Z).OrderBy(g => g.Key))
        //        {
        //            var z = zGroup.Key;

        //            Console.WriteLine($"z={z}");

        //            var activePointsInGroup = zGroup.ToList();

        //            for (var y = dim.MinY; y <= dim.MaxY; y++)
        //            {
        //                for (var x = dim.MinX; x <= dim.MaxX; x++)
        //                {
        //                    Console.Write(activePointsInGroup.Any(p => p.X == x && p.Y == y && p.Z == z) ? '#' : '.');
        //                }

        //                Console.WriteLine();
        //            }
        //        }
        //    //}

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

        public int MaxW{ get; }

        public static Dimensions4D Get(IList<Point4D> space)
        {
            return new(
                space.Min(p => p.X),
                space.Max(p => p.X),
                space.Min(p => p.Y),
                space.Max(p => p.Y),
                space.Min(p => p.Z),
                space.Max(p => p.Z),
                space.Min(p => p.W),
                space.Max(p => p.W)
            );
        }
    }

    class Point4DChange
    {
        public Point4DChange(Point4D point3D, bool isActive)
        {
            Point3D = point3D;
            IsActive = isActive;
        }

        public Point4D Point3D { get; }

        public bool IsActive { get; }
    }
}