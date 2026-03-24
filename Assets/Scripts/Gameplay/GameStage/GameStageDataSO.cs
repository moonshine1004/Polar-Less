using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStageDataSO", menuName = "ScriptableObjects/GameStageDataSO")]
public class GameStageDataSO : ScriptableObject
{
    public int stageID;
    public int levelID;
    public List<Vector3> mirrorData;
}


public class GameStageCatalogSO : ScriptableObject
{
    public List<GameStageDataSO> gameStageDataList;
}