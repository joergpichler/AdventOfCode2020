using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            var seats = new List<Seat>();

            foreach (var line in Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day5.input.txt"))
            {
                var rowId = line.Substring(0, 7);
                var row = GetByBinarySpacePartition(rowId, 'F', 'B');
                var colId = line.Substring(7, 3);
                var col = GetByBinarySpacePartition(colId, 'L', 'R');

                seats.Add(new Seat(row, col));
            }

            Part1(seats);

            Console.WriteLine(1);

            Part2(seats);
        }

        private static void Part1(List<Seat> seats)
        {
            ConsoleHelper.Part1();

            Console.WriteLine($"Highest seat id is: {seats.Max(s => s.Id)}");
        }

        private static void Part2(List<Seat> seats)
        {
            ConsoleHelper.Part2();

            var orderedSeats = seats.OrderBy(s => s.Id).ToList();

            for (var i = 0; i < orderedSeats.Count - 1; i++)
            {
                if (orderedSeats[i + 1].Id == orderedSeats[i].Id + 2)
                {
                    Console.WriteLine($"Id {orderedSeats[i].Id+1} missing");
                    break;
                }
            }
        }

        private static int GetByBinarySpacePartition(string identifier, char lowerChar, char upperChar)
        {
            var total = (int) Math.Pow(2, identifier.Length);

            var space = new Space(total-1);

            foreach(var i in identifier)
            {
                if (i == lowerChar)
                {
                    space.TakeLower();
                }
                else if (i == upperChar)
                {
                    space.TakeUpper();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            if (space.Lower != space.Upper)
            {
                throw new InvalidOperationException("Something went wrong");
            }

            return space.Lower;
        }
    }

    internal class Space
    {
        public Space(int upper)
        {
            Lower = 0;
            Upper = upper;

            if (Range % 2 != 0)
            {
                throw new ArgumentException();
            }
        }

        public int Lower { get; private set; }

        public int Upper { get; private set; }

        private int Range => Upper - Lower + 1;

        public void TakeLower()
        {
            Upper -= Range / 2;
        }

        public void TakeUpper()
        {
            Lower += Range / 2;
        }
    }

    [DebuggerDisplay("{Id}")]
    internal class Seat
    {
        public Seat(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public int Id => Row * 8 + Column;
    }
}
