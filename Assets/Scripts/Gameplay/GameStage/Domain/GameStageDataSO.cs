using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStageDataSO", menuName = "ScriptableObjects/Stage/GameStageDataSO")]
public class GameStageDataSO : ScriptableObject
{
    public int stageID;
    public int levelID;
    public List<ObjectData> objectData = new();
}