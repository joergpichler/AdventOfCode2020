using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lib;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var directions = Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day12.input.txt")
                .Select(x => Direction.Parse(x)).ToList();

            Part1(directions);

            Console.WriteLine();

            Part2(directions);
        }

        private static void Part1(List<Direction> directions)
        {
            var ship = new Ship(new NavigationPt1());

            foreach (var direction in directions)
            {
                ship.Navigate(direction);
            }

            ConsoleHelper.Part1();
            Console.WriteLine($"Manhattan distance of the ship is {Math.Abs(ship.X) + Math.Abs(ship.Y)}");
        }
        
        private static void Part2(List<Direction> directions)
        {
            var ship = new Ship(new NavigationPt2(10, 1));

            foreach (var direction in directions)
            {
                ship.Navigate(direction);
            }

            ConsoleHelper.Part2();
            Console.WriteLine($"Manhattan distance of the ship is {Math.Abs(ship.X) + Math.Abs(ship.Y)}");
        }
    }

    [DebuggerDisplay("{Type} {Value}")]
    internal class Direction
    {
        public Direction(DirectionType type, int value)
        {
            Type = type;
            Value = value;
        }

        public DirectionType Type { get; }

        public int Value { get; }

        public static Direction Parse(string s)
        {
            var match = Regex.Match(s, "^(?<type>N|E|S|W|L|R|F)(?<value>\\d+)$", RegexOptions.Compiled);

            if (!match.Success)
            {
                throw new InvalidOperationException();
            }

            DirectionType directionType;

            switch (match.Groups["type"].Value)
            {
                case "N":
                    directionType = DirectionType.North;
                    break;
                case "E":
                    directionType = DirectionType.East;
                    break;
                case "S":
                    directionType = DirectionType.South;
                    break;
                case "W":
                    directionType = DirectionType.West;
                    break;
                case "L":
                    directionType = DirectionType.Left;
                    break;
                case "R":
                    directionType = DirectionType.Right;
                    break;
                case "F":
                    directionType = DirectionType.Forward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Direction(directionType, int.Parse(match.Groups["value"].Value));
        }
    }

    internal enum DirectionType
    {
        North,
        East,
        South,
        West,
        Forward,
        Left,
        Right
    }


    [DebuggerDisplay("{X} {Y}")]
    internal class Ship
    {
        private readonly INavigation _navigation;

        public Ship(INavigation navigation)
        {
            _navigation = navigation;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public void Navigate(Direction direction)
        {
            _navigation.Navigate(direction, this);
        }
    }

    internal interface INavigation
    {
        void Navigate(Direction direction, Ship ship);
    }

    [DebuggerDisplay("{_orientation}")]
    internal class NavigationPt1 : INavigation
    {
        private int _orientation = 0;

        public void Navigate(Direction direction, Ship ship)
        {
            switch (direction.Type)
            {
                case DirectionType.North:
                    ship.Y += direction.Value;
                    break;
                case DirectionType.East:
                    ship.X += direction.Value;
                    break;
                case DirectionType.South:
                    ship.Y -= direction.Value;
                    break;
                case DirectionType.West:
                    ship.X -= direction.Value;
                    break;
                case DirectionType.Forward:
                    ship.X += (int)(Math.Cos((double)_orientation / 360 * 2 * Math.PI) * direction.Value);
                    ship.Y += (int)(Math.Sin((double)_orientation / 360 * 2 * Math.PI) * direction.Value);
                    break;
                case DirectionType.Left:
                    _orientation += direction.Value;
                    break;
                case DirectionType.Right:
                    _orientation -= direction.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [DebuggerDisplay("{_waypointX}, {_waypointY}")]
    internal class NavigationPt2 : INavigation
    {
        private int _waypointX;
        private int _waypointY;

        public NavigationPt2(int waypointX, int waypointY)
        {
            _waypointX = waypointX;
            _waypointY = waypointY;
        }

        public void Navigate(Direction direction, Ship ship)
        {
            switch (direction.Type)
            {
                case DirectionType.North:
                    _waypointY += direction.Value;
                    break;
                case DirectionType.East:
                    _waypointX += direction.Value;
                    break;
                case DirectionType.South:
                    _waypointY -= direction.Value;
                    break;
                case DirectionType.West:
                    _waypointX -= direction.Value;
                    break;
                case DirectionType.Forward:
                    ship.X += direction.Value * _waypointX;
                    ship.Y += direction.Value * _waypointY;
                    break;
                case DirectionType.Left:
                    RotateWaypoint(direction.Value);
                    break;
                case DirectionType.Right:
                    RotateWaypoint(-direction.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RotateWaypoint(int degrees)
        {
            var hypotenuse = Math.Sqrt(_waypointX * _waypointX + _waypointY * _waypointY);
            var angle = Math.Atan((double) _waypointY / _waypointX);

            // need to correct angle in quadrant 2 & 3
            if (_waypointX < 0)
            {
                angle += Math.PI;
            }

            var newAngle = angle + ((double) degrees / 360 * 2 * Math.PI);

            _waypointX = (int) Math.Round(Math.Cos(newAngle) * hypotenuse);
            _waypointY = (int) Math.Round(Math.Sin(newAngle) * hypotenuse);
        }
    }
}