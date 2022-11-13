using Silk.NET.OpenGL;
using System.Numerics;
using System.Text;

internal sealed class Shader
{
    private const string shaderPath = "assets.shaders";

    private readonly GL _gl;
    private readonly uint _handle;

    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;

        var vertex = LoadShader(ShaderType.VertexShader, vertexPath);
        var fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);

        _handle = _gl.CreateProgram();
        _gl.AttachShader(_handle, vertex);
        _gl.AttachShader(_handle, fragment);
        _gl.LinkProgram(_handle);

        _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
        if (status == 0) throw new Exception($"Shader Linking Error: {_gl.GetProgramInfoLog(_handle)}");

        _gl.DetachShader(_handle, vertex);
        _gl.DetachShader(_handle, fragment);
        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
    }

    public void SetUniform(string name, float value) => _gl.Uniform1(GetUniformLocation(name), value);
    public void SetUniform(string name, Vector3 value) => _gl.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
    public unsafe void SetUniform(string name, Matrix4x4 value) => _gl.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&value);

    public void Use() => _gl.UseProgram(_handle);

    private uint LoadShader(ShaderType type, string path)
    {
        var handle = _gl.CreateShader(type);

        _gl.ShaderSource(handle, Encoding.Default.GetString(Program.GetRes($"{shaderPath}.{path}")));
        _gl.CompileShader(handle);

        var log = _gl.GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(log))
            throw new Exception($"Shader Compilation Error: {path} of type {type}, failed with error {log}");

        return handle;
    }

    private int GetUniformLocation(string name)
    {
        var location = _gl.GetUniformLocation(_handle, name);
        if (location == -1) throw new Exception($"Uniform not found: {name}");
        return location;
    }
}