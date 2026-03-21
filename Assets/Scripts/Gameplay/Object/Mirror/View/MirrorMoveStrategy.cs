using UnityEngine;

public interface IMirrorMoveStrategy
{
    public void Move(Transform transform, Vector2 touchedPosition);
}

public class RotateMirrorMoveStrategy : IMirrorMoveStrategy
{
    public void Move(Transform transform, Vector2 touchedPosition)
    {
        Vector2 mirrorPosition = transform.position;
        Vector2 direction;

        if (touchedPosition.x < mirrorPosition.x)
        {
            direction = touchedPosition - mirrorPosition;
        }
        else
        {
            direction = mirrorPosition - touchedPosition;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

public class SlideMirrorMoveStrategy : IMirrorMoveStrategy
{
    public void Move(Transform transform, Vector2 touchedPosition)
    {
        Vector2 currentPosition = transform.position;

        Vector2 axis = transform.right.normalized;
        Vector2 delta = touchedPosition - currentPosition;

        float distanceOnAxis = Vector2.Dot(delta, axis);
        Vector2 move = axis * distanceOnAxis;
        transform.position = currentPosition + move;
    }
}
