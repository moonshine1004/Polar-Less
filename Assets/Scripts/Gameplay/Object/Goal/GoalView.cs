using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalView : MonoBehaviour, IGetObjectType
{
    [SerializeField] private IGameStageServices _gameStageServices;
    public void Install(IGameStageServices gameStageServices)
    {
        _gameStageServices = gameStageServices;
    }
    private void Awake()
    {
        gameObject.layer = 6;
    }
    public ObjectType GetObjectType()
    {
        return ObjectType.Goal;
    }


}