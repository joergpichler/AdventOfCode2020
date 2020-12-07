using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            var regexOuterBag = new Regex("^(?<color>.+?)\\sbags contain.*", RegexOptions.Compiled);
            var regexInnerBags = new Regex("\\s(?<amount>\\d+)\\s(?<color>.*?)\\sbag", RegexOptions.Compiled);

            var bags = new Dictionary<string, Bag>();

            foreach (var line in Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day7.input.txt"))
            {
                var outerBagMatch = regexOuterBag.Match(line);
                if (!outerBagMatch.Success)
                {
                    throw new InvalidOperationException();
                }

                var outerBagColor = outerBagMatch.Groups["color"].Value;
                Bag outerBag = GetBag(outerBagColor, bags);

                var innerBagsMatches = regexInnerBags.Matches(line);

                foreach (Match innerBagMatch in innerBagsMatches)
                {
                    var amount = int.Parse(innerBagMatch.Groups["amount"].Value);
                    var innerBagColor = innerBagMatch.Groups["color"].Value;

                    var innerBag = GetBag(innerBagColor, bags);

                    for (var i = 0; i < amount; i++)
                    {
                        outerBag.InnerBags.Add(innerBag);
                    }
                }
            }

            Part1(bags.Values);

            Console.WriteLine();

            Part2(bags["shiny gold"]);
        }

        private static void Part2(Bag bag)
        {
            Console.WriteLine($"Amount of bags inside a {bag.Color} bag: {bag.GetNoOfInnerBags()}");
        }

        private static void Part1(IEnumerable<Bag> bags)
        {
            ConsoleHelper.Part1();

            const string bagToCarry = "shiny gold";

            Console.WriteLine($"Amount of bags that can contain a {bagToCarry} bag: {bags.Count(b => b.CanHold(bagToCarry))}");
        }

        private static Bag GetBag(string bagColor, Dictionary<string, Bag> bags)
        {
            if (!bags.TryGetValue(bagColor, out var bag))
            {
                bag = new Bag(bagColor);
                bags.Add(bagColor, bag);
            }

            return bag;
        }
    }

    [DebuggerDisplay("{Color}")]
    internal class Bag
    {
        private static readonly BagEqualityComparer Comparer = new BagEqualityComparer();

        public Bag(string color)
        {
            Color = color ?? throw new ArgumentNullException(nameof(color));
        }

        public string Color { get; }

        public IList<Bag> InnerBags { get; } = new List<Bag>();

        public bool CanHold(string bagColor)
        {
            if(InnerBags.Any(b => b.Color.Equals(bagColor)))
            {
                return true;
            }

            return InnerBags.Distinct(Comparer).Any(b => b.CanHold(bagColor));
        }

        public int GetNoOfInnerBags()
        {
            return InnerBags.Count + InnerBags.Sum(b => b.GetNoOfInnerBags());
        }
    }

    internal class BagEqualityComparer : IEqualityComparer<Bag>
    {
        public bool Equals(Bag x, Bag y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Color == y.Color;
        }

        public int GetHashCode(Bag obj)
        {
            return (obj.Color != null ? obj.Color.GetHashCode() : 0);
        }
    }
}
