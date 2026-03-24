public enum ObjectType
{
    Rotate,
    Slide,
    Static,
    Obstacle,
    Goal
}

public interface IGetObjectType
{
    ObjectType GetObjectType();
}