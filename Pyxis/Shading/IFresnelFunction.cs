namespace Pyxis.Shading
{
    public interface IFresnelFunction
    {
        float Factor(in ShadingInfo info);
    }
}