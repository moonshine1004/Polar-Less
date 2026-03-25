using System.Threading.Tasks;
using UnityEngine;

public interface ILoadStageUseCase
{
    Task ExcuteAsync(GameStageCatalogSO gameStageCatalog, int stageID, int levelID, ObjectFactory objectFactory);
}
public class LoadStageUseCase : ILoadStageUseCase
{
    public async Task ExcuteAsync(GameStageCatalogSO gameStageCatalog, int stageID, int levelID, ObjectFactory objectFactory)
    {
        GameStageDataSO stageData = gameStageCatalog.gameStageDataList.Find(data => data.stageID == stageID && data.levelID == levelID);

        // 오류 방어
        if (stageData == null)
        {
            Debug.LogError($"StageData not found for StageID: {stageID}, LevelID: {levelID}");
            return;
        }
        if (stageData.objectData.Count == 0)
        {
            Debug.LogWarning($"No objects found in StageData for StageID: {stageID}, LevelID: {levelID}");
            return;
        }

        foreach (ObjectData objectData in stageData.objectData)
        {
            objectFactory.CreateObject(objectData);
            await Task.Yield();
        }
    }
}

public interface IClearStageUseCase
{
    void Execute(IRegistMirror mirrorRegistry, IObjectPool<MirrorView> mirrorPool);
}
public class ClearStageUseCase : IClearStageUseCase
{
    public void Execute(IRegistMirror mirrorRegistry, IObjectPool<MirrorView> mirrorPool)
    {
        Debug.Log("Clearing Game Stage...");
        var activedMirrorList = mirrorRegistry.GetActivedMirrorList();
        foreach (MirrorView mirror in activedMirrorList)
        {
            mirrorPool.Return(mirror);
        }
        activedMirrorList.Clear();
    }
}