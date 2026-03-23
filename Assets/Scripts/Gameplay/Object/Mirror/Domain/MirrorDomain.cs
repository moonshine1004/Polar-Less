using UnityEngine;

public enum MirrorType
{
    Rotate,
    Move,
    Static
}

public class MirrorDomain
{
    public MirrorType MirrorType { get; set; }
    public int MirrorID { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public void Init(int ID, Vector3 position, Quaternion rotation)
    {
        MirrorID = ID;
        Position = position;
        Rotation = rotation;
    }
}

