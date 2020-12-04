using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Lib;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            var passports = new List<Passport>();

            StringBuilder sb = new StringBuilder();

            foreach (var line in Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day4.input.txt"))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    passports.Add(Passport.Parse(sb.ToString()));
                    sb.Clear();
                    continue;
                }

                sb.AppendLine(line);
            }

            passports.Add(Passport.Parse(sb.ToString()));

            Part1(passports);

            Console.WriteLine();

            Part2(passports);
        }

        private static void Part1(List<Passport> passports)
        {
            ConsoleHelper.Part1();

            static bool ValidatePassports(Passport p)
            {
                return p.BirthYear.HasValue && p.IssueYear.HasValue && p.ExpirationYear.HasValue && p.Height != null &&
                       p.HairColor != null &&
                       p.EyeColor != null && p.PassportId != null;
            }

            Console.WriteLine($"Valid passports: {passports.Count(ValidatePassports)}");
        }

        private static void Part2(List<Passport> passports)
        {
            ConsoleHelper.Part2();

            static bool ValidatePassports(Passport p)
            {
                var birthYearValid = p.BirthYear.HasValue && p.BirthYear.Value >= 1920 && p.BirthYear.Value <= 2002;
                var issueYearValid = p.IssueYear.HasValue && p.IssueYear.Value >= 2010 && p.IssueYear.Value <= 2020;
                var expirationYearValid = p.ExpirationYear.HasValue && p.ExpirationYear.Value >= 2020 &&
                                          p.ExpirationYear.Value <= 2030;
                var heightValid = false;
                var heightMatch = Regex.Match(p.Height ?? string.Empty, "(?<value>\\d+)(?<unit>(cm|in))");
                if (heightMatch.Success)
                {
                    var value = int.Parse(heightMatch.Groups["value"].Value);
                    switch (heightMatch.Groups["unit"].Value)
                    {
                        case "cm":
                            heightValid = value >= 150 && value <= 193;
                            break;
                        case "in":
                            heightValid = value >= 59 && value <= 76;
                            break;
                    }
                }

                var hairColorValid = Regex.IsMatch(p.HairColor ?? string.Empty, "#[0-9a-f]{6}");

                var eyeColors = new HashSet<string>(new[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"});
                var eyeColorValid = eyeColors.Contains(p.EyeColor ?? string.Empty);

                var passportIdValid = Regex.IsMatch(p.PassportId ?? string.Empty, "^\\d{9}$");

                return birthYearValid && issueYearValid && expirationYearValid && heightValid && hairColorValid &&
                       eyeColorValid && passportIdValid;
            }

            Console.WriteLine($"Valid passports: {passports.Count(ValidatePassports)}");
        }
    }

    internal class Passport
    {
        private Passport()
        {
        }

        // byr
        public int? BirthYear { get; set; }

        // iyr
        public int? IssueYear { get; set; }

        // eyr
        public int? ExpirationYear { get; set; }

        // hgt
        public string Height { get; set; }

        // hcl
        public string HairColor { get; set; }

        // ecl
        public string EyeColor { get; set; }

        // pid
        public string PassportId { get; set; }

        // cid
        public int? CountryId { get; set; }

        private static readonly Regex Regex = new Regex("(?<key>\\S+?):(?<value>\\S+)");

        public static Passport Parse(string s)
        {
            var passport = new Passport();

            var matches = Regex.Matches(s);

            foreach (Match match in matches)
            {
                var value = match.Groups["value"].Value;

                switch (match.Groups["key"].Value)
                {
                    case "byr":
                        passport.BirthYear = int.Parse(value);
                        break;
                    case "iyr":
                        passport.IssueYear = int.Parse(value);
                        break;
                    case "eyr":
                        passport.ExpirationYear = int.Parse(value);
                        break;
                    case "hgt":
                        passport.Height = value;
                        break;
                    case "hcl":
                        passport.HairColor = value;
                        break;
                    case "ecl":
                        passport.EyeColor = value;
                        break;
                    case "pid":
                        passport.PassportId = value;
                        break;
                    case "cid":
                        passport.CountryId = int.Parse(value);
                        break;
                }
            }

            return passport;
        }
    }
}