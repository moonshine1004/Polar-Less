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
    public bool IsControlling { get; set; }
}

