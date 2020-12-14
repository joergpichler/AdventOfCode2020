using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day14.input.txt");

            var instructions = lines.Select(l => Instruction.Parse(l)).ToList();

            Part1(instructions);
        }

        private static void Part1(IEnumerable<Instruction> instructions)
        {
            var machine = new VirtualMachine();

            foreach (var instruction in instructions)
            {
                machine.Execute(instruction);
            }

            ConsoleHelper.Part1();
            Console.WriteLine($"Sum of values in memory: {machine.GetSumOfValuesInMemory()}");
        }
    }

    internal class Instruction
    {
        public Instruction(string bitmask)
        {
            InstructionType = InstructionType.SetBitmask;
            Bitmask = bitmask;
        }

        public Instruction(int location, ulong value)
        {
            InstructionType = InstructionType.SetMemoryValue;
            Location = location;
            Value = value;
        } 

        public InstructionType InstructionType { get; }

        public string Bitmask { get; }

        public int Location { get; }

        public ulong Value { get; }

        public override string ToString()
        {
            switch (InstructionType)
            {
                case InstructionType.SetBitmask:
                    return $"mask = {Bitmask}";
                case InstructionType.SetMemoryValue:
                    return $"mem[{Location}] = {Value}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Instruction Parse(string s)
        {
            var maskMatch = Regex.Match(s, "^mask\\s=\\s(?<mask>.+)$", RegexOptions.Compiled);

            if (maskMatch.Success)
            {
                return new Instruction(maskMatch.Groups["mask"].Value);
            }

            var memoryMatch = Regex.Match(s, "^mem\\[(?<location>\\d+)\\]\\s=\\s(?<value>\\d+)$", RegexOptions.Compiled);

            if (memoryMatch.Success)
            {
                return new Instruction(int.Parse(memoryMatch.Groups["location"].Value), ulong.Parse(memoryMatch.Groups["value"].Value));
            }

            throw new InvalidOperationException();
        }
    }

    internal enum InstructionType
    {
        SetBitmask,
        SetMemoryValue
    }

    internal class VirtualMachine
    {
        private readonly Dictionary<int, ulong> _memory = new Dictionary<int, ulong>();
        private string _bitmask;

        public void Execute(Instruction instruction)
        {
            switch (instruction.InstructionType)
            {
                case InstructionType.SetBitmask:
                    SetBitmask(instruction.Bitmask);
                    break;
                case InstructionType.SetMemoryValue:
                    SetMemoryValue(instruction.Location, instruction.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetBitmask(string bitmask)
        {
            _bitmask = bitmask;
        }

        private void SetMemoryValue(int location, ulong value)
        {
            var maskedValue = ApplyBitmask(value);
            _memory[location] = maskedValue;
        }

        private ulong ApplyBitmask(ulong value)
        {
            var reverseBitmask = new string(_bitmask.Reverse().ToArray());
            
            ulong ones = 0;
            ulong zeroes = ulong.MaxValue;

            for (var i = 0; i < reverseBitmask.Length; i++)
            {
                var c = reverseBitmask[i];
                switch (c)
                {
                    case '1':
                        ones |= ((ulong)1 << i);
                        break;
                    case '0':
                        zeroes &= ~((ulong)1 << i);
                        break;
                    case 'X':
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            value |= ones;
            value &= zeroes;

            return value;
        }

        public BigInteger GetSumOfValuesInMemory()
        {
            BigInteger sum = 0;

            foreach (var value in _memory.Values)
            {
                sum += value;
            }

            return sum;
        }
    }
}
