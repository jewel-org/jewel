using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

internal sealed class Camera
{
    private readonly float _aspectRatio;
    private static Vector2 _lastMousePos;

    public Vector3 Position { get; set; }
    public Vector3 Forward { get; set; } = Vector3.UnitZ;

    public float Zoom { get; set; } = 60;
    public float Yaw { get; set; } = 90;
    public float Pitch { get; set; }

    public float Near { get; set; } = .1f;
    public float Far { get; set; } = 100;

    public float LookSensitivity { get; set; } = .1f;
    public float MoveSpeed { get; set; } = 2;

    public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(Position, Position + Forward, Vector3.UnitY);
    public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(Zoom.Deg2Rad(), _aspectRatio, Near, Far);

    public Camera(Vector3 position)
    {
        Position = position;

        _aspectRatio = (float)Program.Window.Size.X / Program.Window.Size.Y;

        Bind(Program.Window);
    }

    private void Bind(IWindow window)
    {
        window.Update += OnUpdate;

        Input.Mouse.MouseMove += OnMouseMove;
    }

    private void OnUpdate(double delta)
    {
        var speed = (float)delta * MoveSpeed;

        if (Input.Keyboard.IsKeyPressed(Key.S))
            Position -= Forward * speed;
        if (Input.Keyboard.IsKeyPressed(Key.W))
            Position += Forward * speed;
        if (Input.Keyboard.IsKeyPressed(Key.A))
            Position -= Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY)) * speed;
        if (Input.Keyboard.IsKeyPressed(Key.D))
            Position += Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY)) * speed;
    }

    private unsafe void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if (_lastMousePos == default || !mouse.IsButtonPressed(MouseButton.Right))
        {
            _lastMousePos = position;
            return;
        }

        Yaw += (position.X - _lastMousePos.X) * LookSensitivity;
        Pitch -= (position.Y - _lastMousePos.Y) * LookSensitivity;
        Pitch = Math.Clamp(Pitch, -90, 90);

        _lastMousePos = position;

        var dir = Vector3.Zero;
        dir.X = MathF.Cos(Yaw.Deg2Rad()) * MathF.Cos(Pitch.Deg2Rad());
        dir.Y = MathF.Sin(Pitch.Deg2Rad());
        dir.Z = MathF.Sin(Yaw.Deg2Rad()) * MathF.Cos(Pitch.Deg2Rad());
        Forward = Vector3.Normalize(dir);
    }
}