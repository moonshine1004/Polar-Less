using System.Numerics;

public enum MirrorType
{
    Rotate,
    Move,
    Static
}
public struct MirrorData
{
    public MirrorType MirrorType { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
}



