using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObstacleView : MonoBehaviour, IGetObjectType
{
    private void Awake()
    {
        gameObject.layer = 6;
    }
    public ObjectType GetObjectType()
    {
        return ObjectType.Obstacle;
    }
}