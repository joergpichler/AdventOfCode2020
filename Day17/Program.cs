using System;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day17.input.txt")
                .Select(l => l.ToCharArray()).ToList();

            Part1.Run(lines);
            
            Console.WriteLine();

            Part2.Run(lines);
        }
    }
}