using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;

internal static class Renderer
{
    private static readonly GL _gl = GL.GetApi(Program.Window);

    private static readonly ImGuiController _gui;

    private static readonly BufferObject<float> _vbo;
    private static readonly BufferObject<uint> _ebo;
    private static readonly VertexArrayObject<float, uint> _vao;

    private static readonly Texture _texture;

    private static readonly Shader _litShader;
    private static readonly Shader _lampShader;

    private static readonly Camera _cam;

    private static readonly float[] _vertices = { -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f };
    private static readonly uint[] _indices = { 0, 1, 3, 1, 2, 3 };
    private static readonly Vector3 _lampPosition = new(1, 1, 0);

    static Renderer()
    {
        Bind(Program.Window);
        OnResize(Program.Window.Size);

        _gui = new ImGuiController(_gl, Program.Window, Input.Context);

        _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
        _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);

        _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);

        _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 6, 0);
        _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 6, 3);

        _texture = new Texture(_gl, "silk.png");

        _litShader = new Shader(_gl, "default.vert", "lit.frag");
        _lampShader = new Shader(_gl, "default.vert", "default.frag");

        _cam = new Camera(Vector3.UnitZ * -5);
    }

    public static void Init()
    { }

    private static void Bind(IWindow window)
    {
        window.FramebufferResize += OnResize;
        window.Update += OnUpdate;
        window.Render += OnRender;
    }

    private static void OnUpdate(double delta) => _gui.Update((float)delta);

    private static unsafe void OnRender(double delta)
    {
        _gl.Enable(EnableCap.DepthTest);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _vao.Bind();

        _litShader.Use();
        _litShader.SetUniform("uModel", Matrix4x4.CreateRotationY(MathHelper.Deg2Rad(25)));
        _litShader.SetUniform("uView", _cam.ViewMatrix);
        _litShader.SetUniform("uProjection", _cam.ProjectionMatrix);
        _litShader.SetUniform("objectColor", new Vector3(1, .5f, .3f));
        _litShader.SetUniform("lightColor", Vector3.One);
        _litShader.SetUniform("lightPos", _lampPosition);
        _litShader.SetUniform("viewPos", _cam.Position);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        _lampShader.Use();
        _lampShader.SetUniform("uModel", Matrix4x4.Identity * Matrix4x4.CreateScale(.2f) * Matrix4x4.CreateTranslation(_lampPosition));
        _lampShader.SetUniform("uView", _cam.ViewMatrix);
        _lampShader.SetUniform("uProjection", _cam.ProjectionMatrix);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

        _gui.Render();
    }

    private static void OnResize(Vector2D<int> size) => _gl.Viewport(size);
}