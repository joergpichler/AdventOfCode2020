using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Lib;

namespace Day21
{
    class Program
    {
        static void Main(string[] args)
        {
            var foods = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day21.test.input.txt")
                .Select(l => Food.Parse(l)).ToList();

            Part1(foods);
            
            Console.WriteLine();

            Part2(foods);
        }

        private static void Part1(IList<Food> foods)
        {
            var allergensToContainingIngredients = GetAllergensToContainingIngredients(foods);

            var allIngredients = foods.SelectMany(f => f.Ingredients).Distinct();
            var unsafeIngredients = allergensToContainingIngredients.Values.SelectMany(x => x).Distinct();

            var safeIngredients = allIngredients.Except(unsafeIngredients).ToList();
            
            // determine how many times they appear
            var amount = foods.Sum(f => f.Ingredients.Count(i => safeIngredients.Contains(i)));
            
            ConsoleHelper.Part1();
            Console.WriteLine($"Safe ingredients occur {amount} times");
        }

        private static IDictionary<string, IList<string>> GetAllergensToContainingIngredients(IList<Food> foods)
        {
            var allergensToAllIngredients = new Dictionary<string, IList<IList<string>>>();

            // accumulate allergens with ingredients where they occur
            foreach (var food in foods)
            {
                foreach (var allergen in food.Allergens)
                {
                    if (!allergensToAllIngredients.TryGetValue(allergen, out var possibleIngredients))
                    {
                        possibleIngredients = new List<IList<string>>();
                        allergensToAllIngredients.Add(allergen, possibleIngredients);
                    }

                    possibleIngredients.Add(food.Ingredients);
                }
            }

            // determine stuff that definitely contains allergens by accumulating things that occurs in every subentry
            Dictionary<string, IList<string>> allergensToContainingIngredients = new();

            foreach (var kvp in allergensToAllIngredients)
            {
                var ingredientsContainingAllergen = kvp.Value.SelectMany(x => x).GroupBy(x => x)
                    .Where(x => x.Count() == kvp.Value.Count).Select(x => x.Key).ToList();
                allergensToContainingIngredients.Add(kvp.Key, ingredientsContainingAllergen);
            }

            // todo filter list even more
            
            return allergensToContainingIngredients;
        }

        private static void Part2(IList<Food> foods)
        {
            var allergensToContainingIngredients = GetAllergensToContainingIngredients(foods);

            var orderedKeys = allergensToContainingIngredients.Keys.OrderBy(x => x);

            StringBuilder sb = new();

            foreach (var key in orderedKeys)
            {
                sb.Append(allergensToContainingIngredients[key].OrderBy(x => x).Aggregate((a, b) => a + "," + b));
            }
            
            ConsoleHelper.Part2();
            Console.WriteLine(sb);
        }
    }

    class Food
    {
        public Food(IList<string> ingredients, IList<string> allergens)
        {
            Ingredients = ingredients;
            Allergens = allergens;
        }

        public IList<string> Ingredients { get; }
        
        public IList<string> Allergens { get; }

        public static Food Parse(string line)
        {
            var match = Regex.Match(line, "^(?<ingredient>[a-z]+?\\s)+\\(contains\\s(?<allergens>.+)\\)$", RegexOptions.Compiled);

            if (!match.Success)
            {
                throw new InvalidOperationException();
            }

            var ingredients = new List<string>();
            foreach (Capture capture in match.Groups["ingredient"].Captures)
            {
                ingredients.Add(capture.Value.Trim());
            }

            var allergens = match.Groups["allergens"].Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            return new Food(ingredients, allergens);
        }
    }
}
