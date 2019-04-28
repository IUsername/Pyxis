using Pyxis.Texture;

namespace Pyxis.Test.Texture
{
    internal class TestTexture : TextureBase
    {
        public override Color LocalColorAt(in Point localPoint) => new Color(localPoint.X, localPoint.Y, localPoint.Z);
    }
}