using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStageDataSO", menuName = "ScriptableObjects/GameStageDataSO")]
public class GameStageDataSO : ScriptableObject
{
    public int stageID;
    public List<Vector3> mirrorPositions;
}