using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStageDataSO", menuName = "ScriptableObjects/Stage/GameStageCatalogSO")]
public class GameStageCatalogSO : ScriptableObject
{
    public List<GameStageDataSO> gameStageDataList;
}