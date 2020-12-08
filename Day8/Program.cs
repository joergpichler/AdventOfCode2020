using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            var instructions = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day8.input.txt")
                .Select(l => Instruction.Parse(l)).ToArray();

            Part1(instructions);

            Console.WriteLine();

            Part2(instructions);
        }

        private static void Part1(Instruction[] instructions)
        {
            ConsoleHelper.Part1();

            var machine = new VirtualMachine();
            try
            {
                machine.Execute(instructions);
            }
            catch (InfiniteLoopException e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Part2(Instruction[] instructions)
        {
            ConsoleHelper.Part2();

            var machine = new VirtualMachine();
            var programExitedSuccessfully = false;

            for (var line = 0; line < instructions.Length; line++)
            {
                var originalInstruction = instructions[line];

                if (PatchInstruction(originalInstruction, out var patchedInstruction))
                {
                    instructions[line] = patchedInstruction;

                    try
                    {
                        machine.Execute(instructions);
                        programExitedSuccessfully = true;
                    }
                    catch (InfiniteLoopException e)
                    {
                        programExitedSuccessfully = false;
                    }
                }

                instructions[line] = originalInstruction;

                if (programExitedSuccessfully)
                {
                    Console.WriteLine($"Patching line {line} fixed the program");
                    Console.WriteLine($"Accumulator: {machine.Accumulator}");
                    break;
                }
            }
        }

        private static bool PatchInstruction(Instruction instruction, out Instruction patchedInstruction)
        {
            if (instruction.Operation == Operation.Acc)
            {
                patchedInstruction = null;
                return false;
            }
            else
            {
                Operation operation;
                switch (instruction.Operation)
                {
                    case Operation.Nop:
                        operation = Operation.Jmp;
                        break;
                    case Operation.Jmp:
                        operation = Operation.Nop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                patchedInstruction = new Instruction(operation, instruction.Argument);
                return true;
            }
        }
    }

    [DebuggerDisplay("{Operation} {Argument}")]
    internal class Instruction
    {
        public Instruction(Operation operation, int argument)
        {
            Operation = operation;
            Argument = argument;
        }

        public Operation Operation { get; }

        public int Argument { get; }

        public static Instruction Parse(string s)
        {
            var match = Regex.Match(s, "^(?<operation>nop|acc|jmp)\\s(?<argument>(?:\\+|-)\\d+)$", RegexOptions.Compiled);

            if (!match.Success)
            {
                throw new InvalidOperationException();
            }

            return new Instruction(Enum.Parse<Operation>(match.Groups["operation"].Value, true), int.Parse(match.Groups["argument"].Value));
        }
    }

    internal enum Operation
    {
        Acc,
        Jmp,
        Nop
    }

    internal class VirtualMachine
    {
        private readonly HashSet<int> _executedLines = new HashSet<int>();

        public int Pointer { get; private set; }

        public int Accumulator { get; private set; }

        public void Execute(Instruction[] instructions)
        {
            _executedLines.Clear();

            Pointer = 0;
            Accumulator = 0;

            while (true)
            {
                if (Pointer > instructions.Length - 1)
                {
                    Console.WriteLine("Program exited.");
                    break;
                }

                var instruction = instructions[Pointer];

                if (!_executedLines.Add(Pointer))
                {
                    throw new InfiniteLoopException($"Infinite loop detected. Line: {Pointer}. Accumulator: {Accumulator}");
                }

                switch (instruction.Operation)
                {
                    case Operation.Acc:
                        Accumulator += instruction.Argument;
                        Pointer += 1;
                        break;
                    case Operation.Jmp:
                        Pointer += instruction.Argument;
                        break;
                    case Operation.Nop:
                        Pointer += 1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    internal class InfiniteLoopException : Exception
    {
        public InfiniteLoopException(string message) : base(message)
        {
        }
    }
}
