public enum ObjectType
{
    Rotate,
    Slide,
    Static,
    Obstacle,
    SubGoal,
    Goal
}

public interface IGetObjectType
{
    ObjectType GetObjectType();
}