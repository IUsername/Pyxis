using FluentAssertions;
using Xunit;

namespace Octans.Test
{
    public class CanvasTests
    {
        [Fact]
        public void CreateCanvas()
        {
            var c = new Canvas(10, 20);
            c.Width.Should().Be(10);
            c.Height.Should().Be(20);
            c.PixelAt(0, 0).Should().Be(Color.RGB(0, 0, 0));
        }

        [Fact]
        public void CanWriteToPixel()
        {
            var c = new Canvas(10, 20);
            c.WritePixel(Color.RGB(1, 0, 0), 2, 3);
            c.PixelAt(2, 3).Should().Be(Color.RGB(1, 0, 0));
        }
    }
}