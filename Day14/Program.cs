using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
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

            ConsoleHelper.Part1();
            RunVirtualMachine(new VirtualMachineV1(), instructions);

            Console.WriteLine();

            ConsoleHelper.Part2();
            RunVirtualMachine(new VirtualMachineV2(), instructions);
        }

        private static void RunVirtualMachine(VirtualMachine machine, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                machine.Execute(instruction);
            }

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

        public Instruction(ulong location, ulong value)
        {
            InstructionType = InstructionType.SetMemoryValue;
            Location = location;
            Value = value;
        } 

        public InstructionType InstructionType { get; }

        public string Bitmask { get; }

        public ulong Location { get; }

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
                return new Instruction(ulong.Parse(memoryMatch.Groups["location"].Value), ulong.Parse(memoryMatch.Groups["value"].Value));
            }

            throw new InvalidOperationException();
        }
    }

    internal enum InstructionType
    {
        SetBitmask,
        SetMemoryValue
    }

    internal abstract class VirtualMachine
    {
        protected readonly Dictionary<ulong, ulong> Memory = new Dictionary<ulong, ulong>();
        protected string Bitmask;

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

        protected virtual void SetBitmask(string bitmask)
        {
            Bitmask = bitmask;
        }

        protected abstract void SetMemoryValue(ulong location, ulong value);

        protected static ulong ApplyBitmask(ulong value, string bitmask, bool flipZeroes)
        {
            var reverseBitmask = new string(bitmask.Reverse().ToArray());

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
                        if (flipZeroes)
                        {
                            zeroes &= ~((ulong)1 << i);
                        }
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

            foreach (var value in Memory.Values)
            {
                sum += value;
            }

            return sum;
        }
    }

    internal class VirtualMachineV1 : VirtualMachine
    {
        protected override void SetMemoryValue(ulong location, ulong value)
        {
            var maskedValue = ApplyBitmask(value, Bitmask, true);
            Memory[location] = maskedValue;
        }
    }

    internal class VirtualMachineV2 : VirtualMachine
    {
        protected override void SetMemoryValue(ulong location, ulong value)
        {
            var patchedLocation = ApplyBitmask(location, Bitmask, false);

            foreach (var floatingLocation in GetFloatingLocations(patchedLocation, Bitmask))
            {
                Memory[floatingLocation] = value;
            }
        }

        private IEnumerable<ulong> GetFloatingLocations(ulong location, string bitmask)
        {
            var count = bitmask.Count(c => c == 'X');

            foreach (var bits in GetBitVariations(count))
            {
                yield return ApplyBitmask(location, PatchBitmask(Bitmask, bits), true);
            }
        }

        private static IEnumerable<string> GetBitVariations(int depth)
        {
            var maxNum = 1 << (depth);
            for (int i = 0; i < maxNum; i++)
            {
                yield return Convert.ToString(i, 2).PadLeft(depth, '0');
            }
        }

        private string PatchBitmask(string bitmask, string bits)
        {
            var sb = new StringBuilder("".PadLeft(bitmask.Length, '0'));

            int j = 0;
            for (int i = 0; i < bitmask.Length; i++)
            {
                if (bitmask[i] == 'X')
                {
                    sb[i] = bits[j++];
                }
                else
                {
                    sb[i] = 'X';
                }
            }

            return sb.ToString();
        }
    }
}
