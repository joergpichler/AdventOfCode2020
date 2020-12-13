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
                .Where(s => int.TryParse(s, out _)).Select(s => int.Parse(s)).ToList();

            Part1(earliestDeparture, busSchedules);
        }

        private static void Part1(int earliestDeparture, List<int> busSchedules)
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
    }
}
