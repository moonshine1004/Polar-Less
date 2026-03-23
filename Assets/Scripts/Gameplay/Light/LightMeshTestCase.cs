using System.Collections.Generic;
using UnityEngine;

public class LightMeshTestCase : MonoBehaviour
{
    [SerializeField] private LightMesh _lightMesh;
    [SerializeField] private List<Vector3> _pathPoints = new List<Vector3>();

    [ContextMenu("Draw Light Path")]
    public void DrawLightPath()
    {
        _lightMesh.DrawLine(_pathPoints);
    }
}