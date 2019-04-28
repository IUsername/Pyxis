namespace Pyxis.Pipeline
{
    public interface IPixelSamples
    {
        Color GetOrAdd(in SubPixel sp, IPixelRenderer renderer);
        void Reset();
    }
}