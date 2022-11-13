using Silk.NET.OpenGL;

internal sealed class BufferObject<TDataType> where TDataType : unmanaged
{
    private readonly GL _gl;
    private readonly BufferTargetARB _type;
    private readonly uint _handle;

    public unsafe BufferObject(GL gl, ReadOnlySpan<TDataType> data, BufferTargetARB type)
    {
        _gl = gl;
        _type = type;
        _handle = _gl.GenBuffer();
        Bind();

        fixed (void* d = data) _gl.BufferData(type, (nuint)(data.Length * sizeof(TDataType)), d, BufferUsageARB.StaticDraw);
    }

    public void Bind() => _gl.BindBuffer(_type, _handle);
}