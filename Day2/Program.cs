using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var passwordsWithRules = Assembly.GetExecutingAssembly()
                .GetEmbeddedResourceLines("Day2.input.txt")
                .Select(s => PasswordWithRule.Parse(s)).ToArray();

            Part1(passwordsWithRules);

            Console.WriteLine();

            Part2(passwordsWithRules);
        }

        private static void Part1(IEnumerable<PasswordWithRule> passwordsWithRules)
        {
            ConsoleHelper.Part1();

            Console.WriteLine($"{passwordsWithRules.Count(p => p.IsValidPart1())} valid passwords");
        }

        private static void Part2(IEnumerable<PasswordWithRule> passwordsWithRules)
        {
            ConsoleHelper.Part2();

            Console.WriteLine($"{passwordsWithRules.Count(p => p.IsValidPart2())} valid passwords");
        }
    }

    internal static class PasswordWithRuleExtensionMethods
    {
        public static bool IsValidPart1(this PasswordWithRule p)
        {
            var charCount = p.Password.Count(c => c == p.C);

            return charCount >= p.Min && charCount <= p.Max;
        }

        public static bool IsValidPart2(this PasswordWithRule p)
        {
            return (p.Password[p.Min - 1] == p.C && p.Password[p.Max - 1] != p.C) ^ // XOR
                   (p.Password[p.Min - 1] != p.C && p.Password[p.Max - 1] == p.C);
        }
    }

    internal class PasswordWithRule
    {
        private static readonly Regex Regex = new Regex("(?<min>\\d+)-(?<max>\\d+)\\s(?<char>[a-z]):\\s(?<password>[a-z]+)", RegexOptions.Compiled);

        private PasswordWithRule(string password, int min, int max, char c)
        {
            Password = password;
            Min = min;
            Max = max;
            C = c;
        }

        public string Password { get; }

        public int Min { get; }

        public int Max { get; }

        public char C { get; }

        public static PasswordWithRule Parse(string s)
        {
            var match = Regex.Match(s);

            if (!match.Success)
            {
                throw new InvalidOperationException($"Expected to be able to parse '{s}' ");
            }

            return new PasswordWithRule(match.Groups["password"].Value, int.Parse(match.Groups["min"].Value), int.Parse(match.Groups["max"].Value), match.Groups["char"].Value[0]);
        }
    }
}
