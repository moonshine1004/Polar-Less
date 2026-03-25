using UnityEngine;

public class TestCase : MonoBehaviour
{
    [SerializeField] private int _stageID = 0;
    [SerializeField] private int _levelID = 1;
    
    public void Install(IGameStageServices gameStageServices)
    {
        _gameStageServices = gameStageServices;
    }
    [SerializeField] private IGameStageServices _gameStageServices;

    public void Start()
    {
        LoadStage();
    }

    [ContextMenu("LoadStage")]
    public void LoadStage()
    {
        _gameStageServices.LoadGameStageAsync(_stageID, _levelID);
    }

    [ContextMenu("ClearStage")]
    public void ClearStage()
    {
        _gameStageServices.ClearGameStage();
    }

}