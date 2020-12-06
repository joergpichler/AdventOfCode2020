using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Group> groups = new List<Group>();

            var group = new Group();
            groups.Add(group);

            foreach (var line in Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day6.input.txt"))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    group = new Group();
                    groups.Add(group);
                    continue;
                }

                group.Answers.Add(line);
            }

            Part1(groups);

            Console.WriteLine();

            Part2(groups);
        }

        private static void Part1(List<Group> groups)
        {
            Console.WriteLine($"Sum of counts is: {groups.Sum(g => g.GetDistinctQuestions().Count())}");
        }

        private static void Part2(List<Group> groups)
        {
            Console.WriteLine($"Sum of counts is: {groups.Sum(g => g.GetQuestionsThatAllAnswered().Count())}");
        }
    }

    internal class Group
    {
        public List<string> Answers { get; } = new List<string>();

        public IEnumerable<char> GetDistinctQuestions()
        {
            var hashSet = new HashSet<char>();

            foreach (var answer in Answers)
            {
                foreach (var c in answer)
                {
                    hashSet.Add(c);
                }
            }

            return hashSet;
        }

        public IEnumerable<char> GetQuestionsThatAllAnswered()
        {
            var distinctQuestions = GetDistinctQuestions();

            foreach (var distinctQuestion in distinctQuestions)
            {
                if (Answers.All(a => a.Contains(distinctQuestion)))
                {
                    yield return distinctQuestion;
                }
            }
        }
    }

    
}
