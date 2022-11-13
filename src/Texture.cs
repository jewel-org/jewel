using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

internal sealed class Texture
{
    private const string texturePath = "assets.textures";

    private readonly GL _gl;
    private readonly uint _handle;

    public unsafe Texture(GL gl, string path)
    {
        _gl = gl;
        _handle = _gl.GenTexture();

        var image = Image.Load<Rgba32>(Program.GetRes($"{texturePath}.{path}"));
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        var data = new byte[4 * image.Width * image.Height];
        image.CopyPixelDataTo(data);

        CreateTexture(data, (uint)image.Width, (uint)image.Height);
    }

    public unsafe Texture(GL gl, ReadOnlySpan<byte> data, uint width, uint height)
    {
        _gl = gl;
        _handle = _gl.GenTexture();

        CreateTexture(data, width, height);
    }

    private unsafe void CreateTexture(ReadOnlySpan<byte> data, uint width, uint height)
    {
        Bind();

        fixed (void* d = &data[0]) _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);

        SetParameters();
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }

    private void SetParameters()
    {
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)GLEnum.LinearMipmapLinear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }
}