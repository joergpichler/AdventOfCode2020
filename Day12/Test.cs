using NUnit.Framework;

namespace Day12
{
    public class Test
    {
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        [TestCase(1, -1)]
        public void TestNavigationPt2(int x, int y)
        {
            var ship = new Ship(new NavigationPt2(x, y));

            ship.Navigate(new Direction(DirectionType.Forward, 10));
            ship.Navigate(new Direction(DirectionType.Left, 180));
            ship.Navigate(new Direction(DirectionType.Forward, 10));
            ship.Navigate(new Direction(DirectionType.Forward, 10));
            ship.Navigate(new Direction(DirectionType.Right, 180));
            ship.Navigate(new Direction(DirectionType.Forward, 10));

            Assert.That(ship.X, Is.EqualTo(0));
            Assert.That(ship.Y, Is.EqualTo(0));
        }
    }
}