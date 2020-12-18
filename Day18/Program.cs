using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            var expressions = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day18.input.txt").ToList();

            Part1(expressions);
            
            Console.WriteLine();

            Part2(expressions);
        }

        private static void Part1(IEnumerable<string> expressions)
        {
            ConsoleHelper.Part1();
            var solver = new Pt1MathExpressionSolver();
            Console.WriteLine($"Sum: {expressions.Select(e => solver.Solve(e)).Sum()}");
        }

        private static void Part2(IEnumerable<string> expressions)
        {
            ConsoleHelper.Part2();
            var solver = new Pt2MathExpressionSolver();
            Console.WriteLine($"Sum: {expressions.Select(e => solver.Solve(e)).Sum()}");
        }
    }

    interface IMathExpressionSolver
    {
        long Solve(string mathExpression);
    }

    class Pt1MathExpressionSolver : IMathExpressionSolver
    {
        public long Solve(string mathExpression)
        {
            var originalMathExpression = mathExpression;

            var regexOperation = new Regex("(?<a>\\d+)\\s?(?<op>\\+|\\*)\\s?(?<b>\\d+)", RegexOptions.Compiled);
            var regexNumberInBraces = new Regex("\\((?<number>\\d+)\\)", RegexOptions.Compiled);
            
            while (mathExpression.Any(c => c == '+' || c == '*'))
            {
                mathExpression = regexOperation.Replace(mathExpression,
                    match =>
                    {
                        long a = long.Parse(match.Groups["a"].Value);
                        long b = long.Parse(match.Groups["b"].Value);
                        long result;
                        switch (match.Groups["op"].Value)
                        {
                            case "+":
                                result = a + b;
                                break;
                            case "*":
                                result = a * b;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        return result.ToString();
                    }, 1);
                mathExpression = regexNumberInBraces.Replace(mathExpression, match => match.Groups["number"].Value);
            }

            return long.Parse(mathExpression);
        }
    }

    class Pt2MathExpressionSolver : IMathExpressionSolver
    {
        public long Solve(string mathExpression)
        {
            var originalMathExpression = mathExpression;

            var regexAddition = new Regex("(?<a>\\d+)\\s?(?<op>\\+)\\s?(?<b>\\d+)", RegexOptions.Compiled);
            var regexMultiplicationInBraces = new Regex("\\((?<a>\\d+)\\s?\\*(?:(?<a>\\d+)|[\\s\\*])*\\)", RegexOptions.Compiled);
            var regexMultiplicationWithoutBraces =
                new Regex("(?<a>\\d+)\\s?\\*(?:(?<a>\\d+)|[\\s\\*])*", RegexOptions.Compiled);
            var regexNumberInBraces = new Regex("\\((?<number>\\d+)\\)", RegexOptions.Compiled);

            while (mathExpression.Any(c => c == '+') || regexMultiplicationInBraces.IsMatch(mathExpression))
            {
                var tmpExpression = regexAddition.Replace(mathExpression, EvaluateOperation, 1);
                while (!tmpExpression.Equals(mathExpression))
                {
                    mathExpression = tmpExpression;
                    tmpExpression = regexAddition.Replace(mathExpression, EvaluateOperation, 1);
                }
                
                mathExpression = regexNumberInBraces.Replace(mathExpression, match => match.Groups["number"].Value);

                while (regexMultiplicationInBraces.IsMatch(mathExpression))
                {
                    mathExpression = regexMultiplicationInBraces.Replace(mathExpression, EvaluateMultiplication, 1);
                }
            }

            while (regexMultiplicationWithoutBraces.IsMatch(mathExpression))
            {
                mathExpression = regexMultiplicationWithoutBraces.Replace(mathExpression, EvaluateMultiplication, 1);
            }

            return long.Parse(mathExpression);
        }

        private string EvaluateMultiplication(Match match)
        {
            long result = 1;
            foreach (Capture capture in match.Groups["a"].Captures)
            {
                result *= long.Parse(capture.Value);
            }

            return result.ToString();
        }

        private string EvaluateOperation(Match match)
        {
            long a = long.Parse(match.Groups["a"].Value);
            long b = long.Parse(match.Groups["b"].Value);
            long result;
            switch (match.Groups["op"].Value)
            {
                case "+":
                    result = a + b;
                    break;
                case "*":
                    result = a * b;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result.ToString();
        }
    }
}