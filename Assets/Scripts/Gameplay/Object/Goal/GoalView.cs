using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalView : MonoBehaviour, IGetObjectType
{
    
    private void Awake()
    {
        gameObject.layer = 6;

    }
    public ObjectType GetObjectType()
    {
        return ObjectType.Goal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<LightView>() != null)
        {
            Debug.Log("Goal Reached!");
        }
    }
}