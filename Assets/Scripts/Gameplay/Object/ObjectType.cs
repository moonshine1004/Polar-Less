public enum ObjectType
{
    Rotate,
    Move,
    Static,
    Obstacle,
    Goal
}

public interface IGetObjectType
{
    ObjectType GetObjectType();
}