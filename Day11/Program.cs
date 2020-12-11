using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lib;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var seatLayout =
                SeatLayout.Parse(Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day11.input.txt"));

            Part1(seatLayout);

            Console.WriteLine();

            // need to reset seat layout, taking the easy way here
            seatLayout =
                SeatLayout.Parse(Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day11.input.txt"));

            Part2(seatLayout);
        }

        private static void Part1(SeatLayout seatLayout)
        {
            var changes = SeatChangeCalculator.CalculateChangesPart1(seatLayout).ToList();

            while (changes.Any())
            {
                seatLayout.ApplyChanges(changes);

                changes = SeatChangeCalculator.CalculateChangesPart1(seatLayout).ToList();
            }

            ConsoleHelper.Part1();

            Console.WriteLine($"Occupied seats: {seatLayout.Enumerate().Count(s => s == TileType.SeatOccupied)}");
        }

        private static void Part2(SeatLayout seatLayout)
        {
            var changes = SeatChangeCalculator.CalculateChangesPart2(seatLayout).ToList();

            while (changes.Any())
            {
                seatLayout.ApplyChanges(changes);

                changes = SeatChangeCalculator.CalculateChangesPart2(seatLayout).ToList();
            }

            ConsoleHelper.Part2();

            Console.WriteLine($"Occupied seats: {seatLayout.Enumerate().Count(s => s == TileType.SeatOccupied)}");
        }
    }

    internal class SeatLayout
    {
        private readonly TileType[,] _tiles;

        public SeatLayout(TileType[,] tiles)
        {
            _tiles = tiles;
        }

        public int DimX => _tiles.GetLength(0);

        public int DimY => _tiles.GetLength(1);

        public bool TryGetTile(int x, int y, out TileType tileType)
        {
            if (x >= 0 && x < _tiles.GetLength(0) && y >= 0 && y < _tiles.GetLength(1))
            {
                tileType = _tiles[x, y];
                return true;
            }

            tileType = default;
            return false;
        }

        public void ToggleSeatOccupancy(int x, int y)
        {
            _tiles[x, y] = _tiles[x, y] switch
            {
                TileType.SeatEmpty => TileType.SeatOccupied,
                TileType.SeatOccupied => TileType.SeatEmpty,
                _ => throw new InvalidOperationException()
            };
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var y = 0; y < _tiles.GetLength(1); y++)
            {
                for (var x = 0; x < _tiles.GetLength(0); x++)
                {
                    sb.Append(_tiles[x, y].ToChar());
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static SeatLayout Parse(IEnumerable<string> lines)
        {
            var tiles = lines.Select(l => l.Select(c =>
            {
                switch (c)
                {
                    case '.':
                        return TileType.Floor;
                    case 'L':
                        return TileType.SeatEmpty;
                    case '#':
                        return TileType.SeatOccupied;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).ToArray()).ToArray();

            return new SeatLayout(tiles.To2DArray());
        }
    }

    internal class SeatChange
    {
        public SeatChange(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }
    }

    internal class SeatChangeCalculator
    {
        public static IEnumerable<SeatChange> CalculateChangesPart1(SeatLayout seatLayout)
        {
            for (var x = 0; x < seatLayout.DimX; x++)
            {
                for (var y = 0; y < seatLayout.DimY; y++)
                {
                    if(!seatLayout.TryGetTile(x, y, out var tile))
                    {
                        throw new InvalidOperationException();
                    }

                    if (tile == TileType.Floor)
                    {
                        continue;
                    }

                    var adjacentSeats = seatLayout.GetAdjacentTiles(x, y);

                    if (tile == TileType.SeatEmpty && !adjacentSeats.Any(s => s == TileType.SeatOccupied))
                    {
                        yield return new SeatChange(x, y);
                    }

                    if (tile == TileType.SeatOccupied && adjacentSeats.Count(s => s == TileType.SeatOccupied) >= 4)
                    {
                        yield return new SeatChange(x, y);
                    }
                }
            }
        }

        public static IEnumerable<SeatChange> CalculateChangesPart2(SeatLayout seatLayout)
        {
            for (var x = 0; x < seatLayout.DimX; x++)
            {
                for (var y = 0; y < seatLayout.DimY; y++)
                {
                    if (!seatLayout.TryGetTile(x, y, out var tile))
                    {
                        throw new InvalidOperationException();
                    }

                    if (tile == TileType.Floor)
                    {
                        continue;
                    }

                    var seatsInSight = seatLayout.GetSeatsInSigth(x, y);

                    if (tile == TileType.SeatEmpty && !seatsInSight.Any(s => s == TileType.SeatOccupied))
                    {
                        yield return new SeatChange(x, y);
                    }

                    if (tile == TileType.SeatOccupied && seatsInSight.Count(s => s == TileType.SeatOccupied) >= 5)
                    {
                        yield return new SeatChange(x, y);
                    }
                }
            }
        }
    }

    internal enum TileType
    {
        Floor,
        SeatEmpty,
        SeatOccupied
    }

    internal static class Extensions
    {
        public static T[,] To2DArray<T>(this T[][] array)
        {
            var xDim = array[0].Length;

            if (array.Any(a => a.Length != xDim))
            {
                throw new InvalidOperationException();
            }

            var yDim = array.Length;

            var result = new T[xDim, yDim];

            for (var y = 0; y < yDim; y++)
            {
                for (var x = 0; x < xDim; x++)
                {
                    result[x, y] = array[y][x];
                }
            }

            return result;
        }

        public static char ToChar(this TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Floor:
                    return '.';
                case TileType.SeatEmpty:
                    return 'L';
                case TileType.SeatOccupied:
                    return '#';
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null);
            }
        }

        public static IEnumerable<TileType> GetAdjacentTiles(this SeatLayout seatLayout, int x, int y)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    if (seatLayout.TryGetTile(x + dx, y + dy, out var tile))
                    {
                        yield return tile;
                    }
                }
            }
        }

        public static IEnumerable<TileType> GetSeatsInSigth(this SeatLayout seatLayout, int x, int y)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int ctr = 1;
                    TileType tile;

                    while (seatLayout.TryGetTile(x + ctr * dx, y + ctr * dy, out tile) && tile == TileType.Floor)
                    {
                        ctr += 1;
                    }

                    if (tile != TileType.Floor)
                    {
                        yield return tile;
                    }
                }
            }
        }

        public static void ApplyChanges(this SeatLayout seatLayout, IEnumerable<SeatChange> changes)
        {
            foreach (var change in changes)
            {
                seatLayout.ToggleSeatOccupancy(change.X, change.Y);
            }
        }

        public static IEnumerable<TileType> Enumerate(this SeatLayout seatLayout)
        {
            for (var y = 0; y < seatLayout.DimY; y++)
            {
                for (var x = 0; x < seatLayout.DimX; x++)
                {
                    if (seatLayout.TryGetTile(x, y, out var tile))
                    {
                        yield return tile;
                    }
                }
            }
        }
    }
}