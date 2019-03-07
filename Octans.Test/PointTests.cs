using FluentAssertions;
using Xunit;

namespace Octans.Test
{
    public class PointTests
    {
        [Fact]
        public void WOfOneIsPoint()
        {
            var a = new Point(4.3f, -4.2f, 3.1f);
            a.X.Should().Be(4.3f);
            a.Y.Should().Be(-4.2f);
            a.Z.Should().Be(3.1f);
            a.W.Should().Be(1.0f);
        }

        [Fact]
        public void CanAddVectorToPoint()
        {
            var a1 = new Point(3, -2, 5);
            var a2 = new Vector(-2, 3, 1);
            (a1 + a2).Should().BeEquivalentTo(new Point(1, 1, 6));
        }

        [Fact]
        public void SubtractingTwoPoints()
        {
            var p1 = new Point(3, 2, 1);
            var p2 = new Point(5, 6, 7);
            (p1 - p2).Should().BeEquivalentTo(new Vector(-2, -4, -6));
        }

        [Fact]
        public void SubtractingVectorFromPoint()
        {
            var p = new Point(3, 2, 1);
            var v = new Vector(5, 6, 7);
            (p - v).Should().BeEquivalentTo(new Point(-2, -4, -6));
        }

        [Fact]
        public void NegatePoint()
        {
            var a = new Point(1, -2, 3, 4);
            (-a).Should().BeEquivalentTo(new Point(-1, 2, -3, -4));
        }

        [Fact]
        public void MultiplyingByScalar()
        {
            var a = new Point(1, -2, 3, -4);
            (a * 3.5f).Should().BeEquivalentTo(new Point(3.5f, -7, 10.5f, -14));
        }

        [Fact]
        public void MultiplyingByFraction()
        {
            var a = new Point(1, -2, 3, -4);
            (a * 0.5f).Should().BeEquivalentTo(new Point(0.5f, -1, 1.5f, -2));
        }

        [Fact]
        public void DividingByScalar()
        {
            var a = new Point(1, -2, 3, -4);
            (a / 2).Should().BeEquivalentTo(new Point(0.5f, -1, 1.5f, -2));
        }
    }
}