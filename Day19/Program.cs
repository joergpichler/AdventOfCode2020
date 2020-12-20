using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day19
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = Parse(Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day19.input.txt"));

            Part1(data.ruleBuilder.Build(0, pattern => "^" + pattern + "$"), data.messages);
            
            Console.WriteLine();

            Part2(data.ruleBuilder, data.messages);
        }

        private static void Part1(Regex regex, IEnumerable<string> messages)
        {
            ConsoleHelper.Part1();
            Console.WriteLine($"No of matching messages: {messages.Count(regex.IsMatch)}");
        }

        private static void Part2(RegexBuilder regexBuilder, IEnumerable<string> messages)
        {
            regexBuilder.Add("8: 42+");
            // https://stackoverflow.com/a/17004406
            regexBuilder.Add("11: (?<open>42)+(?<-open>31)+(?(open)(?!))");

            var regex = regexBuilder.Build(0, pattern => "^" + pattern + "$");
            
            ConsoleHelper.Part2();
            Console.WriteLine($"No of matching messages: {messages.Count(regex.IsMatch)}");
        }

        private static (RegexBuilder ruleBuilder, IEnumerable<string> messages) Parse(IEnumerable<string> input)
        {
            using var enumerator = input.GetEnumerator();

            var regexBuilder = new RegexBuilder();
            while (enumerator.MoveNext() && !string.IsNullOrWhiteSpace(enumerator.Current))
            {
                regexBuilder.Add(enumerator.Current);
            }

            var messages = new List<string>();
            while (enumerator.MoveNext() && !string.IsNullOrWhiteSpace(enumerator.Current))
            {
                messages.Add(enumerator.Current);
            }

            return (regexBuilder, messages);
        }
    }

    class RegexBuilder
    {
        private readonly Dictionary<int, string> _rules = new();

        public void Add(string s)
        {
            var split = s.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var ruleNr = int.Parse(split[0]);
            var ruleContent = split[1];
            
            _rules[ruleNr] = ruleContent.TrimStart('"').TrimEnd('"'); // Trim characters
        }

        public Regex Build(int ruleNr, Func<string, string> patternEvaluator = null)
        {
            string pattern = _rules[ruleNr];

            var numberRegex = new Regex("\\d+", RegexOptions.Compiled);

            var match = numberRegex.Match(pattern);
            while (match.Success)
            {
                pattern = numberRegex.Replace(pattern, matchEvaluator =>
                {
                    var ruleNo = int.Parse(matchEvaluator.Value);
                    var rule = _rules[ruleNo];
                    
                    if (numberRegex.IsMatch(rule))
                    {
                        return "(?:" + rule + ")";
                    }
                    else
                    {
                        return rule;
                    }
                }, 1);

                match = numberRegex.Match(pattern);
            }

            pattern = Regex.Replace(pattern, "\\s", string.Empty);
            if (patternEvaluator != null)
            {
                pattern = patternEvaluator(pattern);
            }
            return new Regex(pattern, RegexOptions.Compiled);
        }
    }
}