using System.Numerics;

internal sealed class Transform
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public float Scale { get; set; } = 1;

    public Matrix4x4 Matrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
}