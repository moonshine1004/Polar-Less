using System.Threading.Tasks;
using UnityEngine;

public interface IGameStageServices
{
    void LoadGameStageAsync(int stageID, int levelID);
    void ClearGameStage();
}

public class GameStageServices : IGameStageServices
{
    public GameStageServices(GameStageCatalogSO gameStageCatalog, ObjectFactory objectFactory, IObjectPool<MirrorView> mirrorPool, IRegistMirror mirrorRegistry)
    {
        _gameStageCatalog = gameStageCatalog;
        _objectFactory = objectFactory;
        _mirrorPool = mirrorPool;
        _mirrorRegistry = mirrorRegistry;
    }

    public ILoadStageUseCase _loadStageUseCase = new LoadStageUseCase();
    public IClearStageUseCase _clearStageUseCase = new ClearStageUseCase();

    private readonly GameStageCatalogSO _gameStageCatalog;
    private readonly IObjectPool<MirrorView> _mirrorPool;
    private readonly IRegistMirror _mirrorRegistry;
    private readonly ObjectFactory _objectFactory;

    public void LoadGameStageAsync(int stageID, int levelID)
    {
        _loadStageUseCase.ExcuteAsync(_gameStageCatalog, stageID, levelID, _objectFactory);
    }
    public void ClearGameStage()
    {
        _clearStageUseCase.Execute(_mirrorRegistry, _mirrorPool);
        Debug.Log("Game Stage Cleared.");
    }



    
}