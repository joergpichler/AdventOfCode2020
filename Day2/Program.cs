using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var passwordsWithRules = new List<PasswordWithRule>();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Day2.input.txt"))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        passwordsWithRules.Add(PasswordWithRule.Parse(line));
                    }
                }
            }

            Part1(passwordsWithRules);
            Console.WriteLine();
            Part2(passwordsWithRules);
        }

        private static void Part1(List<PasswordWithRule> passwordsWithRules)
        {
            Console.WriteLine("PART 1");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"{passwordsWithRules.Count(p => p.IsValidPart1())} valid passwords");
        }

        private static void Part2(List<PasswordWithRule> passwordsWithRules)
        {
            Console.WriteLine("PART 2");
            Console.WriteLine("---------------------------");
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
            return (p.Password[p.Min - 1] == p.C && p.Password[p.Max - 1] != p.C) ||
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
