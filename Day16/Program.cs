using System;
using System.Collections.Generic;
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
        }

        private static void Part1(IEnumerable<Rule> rules, IEnumerable<Ticket> tickets)
        {
            int errorRate = tickets.Sum(ticket => ticket.Numbers.Where(n => !IsNumberValid(n, rules)).Sum());

            ConsoleHelper.Part1();
            Console.WriteLine($"Ticket scanning error rate: {errorRate}");
        }

        private static bool IsNumberValid(int number, IEnumerable<Rule> rules)
        {
            return rules.SelectMany(rule => rule.Ranges).Any(range => range.From <= number && range.To >= number);
        }

        internal class Rule
        {
            public Rule(string field, Range[] ranges)
            {
                Field = field;
                Ranges = ranges;
            }

            public string Field { get; }

            public IEnumerable<Range> Ranges { get; }
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

            public IEnumerable<int> Numbers { get; }

            public static Ticket Parse(string s)
            {
                return new(s.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
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
