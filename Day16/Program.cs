using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = Parse(Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day16.input.txt"));

            Part1(data.rules, data.tickets);
            
            Console.WriteLine();

            Part2(data.rules, data.myTicket, data.tickets);
        }

        private static void Part1(IEnumerable<Rule> rules, IEnumerable<Ticket> tickets)
        {
            int errorRate = 0;
            
            foreach (var invalidTicket in tickets.Where(t => !t.IsValid(rules)))
            {
                var invalidNumbers = invalidTicket.Numbers.Where(n => rules.All(r => !r.IsNumberValid(n))).Sum();
                errorRate += invalidNumbers;
            }

            ConsoleHelper.Part1();
            Console.WriteLine($"Ticket scanning error rate: {errorRate}");
        }

        private static void Part2(IEnumerable<Rule> rules, Ticket myTicket, IEnumerable<Ticket> tickets)
        {
            var validTickets = new List<Ticket>() { myTicket };
            
            validTickets.AddRange(tickets.Where(t => t.IsValid(rules)));

            var rulesForColumns = GetRulesMatchingToColumns(rules, validTickets);

            List<int> relevantNumbersOnTicket = new();
            
            for (int i = 0; i < rulesForColumns.Length; i++)
            {
                var rule = rulesForColumns[i];

                if (!rule.Field.StartsWith("departure"))
                {
                    continue;
                }
                
                relevantNumbersOnTicket.Add(myTicket.Numbers[i]);
            }
            
            ConsoleHelper.Part2();
            Console.WriteLine($"Multiplication result: {relevantNumbersOnTicket.Select(n => (ulong) n).Aggregate((a, b) => a * b)}");
        }

        private static Rule[] GetRulesMatchingToColumns(IEnumerable<Rule> rules, List<Ticket> validTickets)
        {
            var myTicket = validTickets[0];
            var rulesForColumns = new Rule[myTicket.Numbers.Length];

            var rulesToDistribute = rules.ToList();

            while (rulesToDistribute.Any())
            {
                for (var i = 0; i < myTicket.Numbers.Length; i++)
                {
                    if (rulesForColumns[i] != null)
                    {
                        continue;
                    }

                    var numbersAtPosInAllTickets = validTickets.Select(t => t.Numbers[i]);

                    var matchingRules = FindMatchingRules(numbersAtPosInAllTickets, rulesToDistribute).ToList();

                    if (matchingRules.Count == 1)
                    {
                        var matchingRule = matchingRules.Single();
                        rulesForColumns[i] = matchingRule;
                        rulesToDistribute.Remove(matchingRule);
                    }
                }
            }

            return rulesForColumns;
        }

        private static IEnumerable<Rule> FindMatchingRules(IEnumerable<int> numbers, IEnumerable<Rule> rules)
        {
            return rules.Where(r => numbers.All(n => r.IsNumberValid(n)));
        }

        [DebuggerDisplay("{Field}")]
        internal class Rule
        {
            public Rule(string field, Range[] ranges)
            {
                Field = field;
                Ranges = ranges;
            }

            public string Field { get; }

            public IEnumerable<Range> Ranges { get; }

            public bool IsNumberValid(int number)
            {
                return Ranges.Any(range => range.From <= number && range.To >= number);
            }
        }

        internal class Range
        {
            public Range(int @from, int to)
            {
                From = @from;
                To = to;
            }

            public int From { get; }

            
            public int To { get; }

            public static Range Parse(string s)
            {
                var split = s.Split('-', StringSplitOptions.RemoveEmptyEntries);
                return new Range(int.Parse(split[0]), int.Parse(split[1]));
            }
        }

        internal class Ticket
        {
            public Ticket(int[] numbers)
            {
                Numbers = numbers;
            }

            public int[] Numbers { get; }

            public static Ticket Parse(string s)
            {
                return new(s.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            }

            public bool IsValid(IEnumerable<Rule> rules)
            {
                return Numbers.All(n => rules.Any(r => r.IsNumberValid(n)));
            }
        }
        
        private static (IEnumerable<Rule> rules, Ticket myTicket, IEnumerable<Ticket> tickets) Parse(IEnumerable<string> input)
        {
            using var enumerator = input.GetEnumerator();

            // first block rules
            var rules = new List<Rule>();
            while (enumerator.MoveNext() && !string.IsNullOrWhiteSpace(enumerator.Current))
            {
                var match = Regex.Match(enumerator.Current, "^(?<field>.+?):\\s(?<rule>\\d+-\\d+)(?:\\sor\\s(?<rule>\\d+-\\d+))$", RegexOptions.Compiled);
                if (!match.Success)
                {
                    throw new InvalidOperationException();
                }
                rules.Add(new Rule(
                    match.Groups["field"].Value,
                    match.Groups["rule"].Captures.Select(c => Range.Parse(c.Value)).ToArray()
                    ));
            }

            // second block my ticket
            Ticket myTicket = null;
            while (enumerator.MoveNext() && !string.IsNullOrWhiteSpace(enumerator.Current))
            {
                if (enumerator.Current.Equals("your ticket:"))
                {
                    enumerator.MoveNext();
                    myTicket = Ticket.Parse(enumerator.Current);
                }
            }

            if (myTicket == null)
            {
                throw new InvalidOperationException();
            }

            // third block other tickets
            var tickets = new List<Ticket>();
            while (enumerator.MoveNext() && !string.IsNullOrWhiteSpace(enumerator.Current))
            {
                if (enumerator.Current.Equals("nearby tickets:"))
                {
                    continue;
                }
                
                tickets.Add(Ticket.Parse(enumerator.Current));
            }

            return (rules, myTicket, tickets);
        }
    }
}
