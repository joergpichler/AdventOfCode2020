using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day13.input.txt").ToList();

            var earliestDeparture = int.Parse(lines[0]);
            var busSchedules = lines[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var x) ? x : -1).ToArray();

            Part1(earliestDeparture, busSchedules.Where(x => x != -1));

            Console.WriteLine();

            Part2(busSchedules);
        }

        private static void Part1(int earliestDeparture, IEnumerable<int> busSchedules)
        {
            var scheduleAndRemainder = busSchedules.Select(b =>
            {
                // calculate integer division and multiply again to get nearest number
                var div = earliestDeparture / b;
                var mult = div * b;
                // if number is smaller than target, add it again once 
                if (mult < earliestDeparture)
                {
                    mult += b;
                }

                return (busSchedule: b, departure: mult);
            }).ToList();

            var min = scheduleAndRemainder.OrderBy(x => x.departure).First();
            var waitTime = min.departure - earliestDeparture;

            ConsoleHelper.Part1();
            Console.WriteLine($"Bus ID: {min.busSchedule} Departure: {min.departure} Wait time: {waitTime} Result: {min.busSchedule * waitTime}");
        }

        private static void Part2(IReadOnlyList<int> busSchedules)
        {
            IList<(int index, int busSchedule)> schedules = new List<(int index, int busSchedule)>();

            for (var i = 0; i < busSchedules.Count; i++)
            {
                var busSchedule = busSchedules[i];
                if (busSchedule == -1)
                {
                    continue;
                }
                schedules.Add((i, busSchedule));
            }

            ulong time = 0;
            ulong multiplier = (ulong) schedules[0].busSchedule;
            int satisfied = 1;

            while (satisfied < schedules.Count)
            {
                time += multiplier;
                var next = schedules[satisfied];

                if ((time + (ulong) next.index) % (ulong)next.busSchedule == 0)
                {
                    multiplier *= (ulong) next.busSchedule;
                    satisfied++;
                }
            }
            
            Console.WriteLine($"Timestamp: {time}");
        }
    }
}
