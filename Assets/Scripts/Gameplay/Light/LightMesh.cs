using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LightMesh : MonoBehaviour
{
    [SerializeField] private float _width = 0.3f;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private List<Vector3> _vertices;
    private List<Vector2> _uvs;
    private List<int> _triangles;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();

        _mesh = new Mesh();
        _mesh.name = "LightMesh";
        _meshFilter.sharedMesh = _mesh;

        _vertices = new List<Vector3>();
        _uvs = new List<Vector2>();
        _triangles = new List<int>();
    }

    public void DrawLine(List<Vector3> pathPoints)
    {
        ClearPath();

        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector2 normal;

            if (i == 0)
            {
                Vector2 dir = (pathPoints[1] - pathPoints[0]).normalized;
                normal = GetNormal(dir);
            }
            else if (i == pathPoints.Count - 1)
            {
                Vector2 dir = (pathPoints[i] - pathPoints[i - 1]).normalized;
                normal = GetNormal(dir);
            }
            else
            {
                Vector2 dirPrev = (pathPoints[i] - pathPoints[i - 1]).normalized;
                Vector2 dirNext = (pathPoints[i + 1] - pathPoints[i]).normalized;

                Vector2 normalPrev = GetNormal(dirPrev);
                Vector2 normalNext = GetNormal(dirNext);

                normal = (normalPrev + normalNext).normalized;

                float dot = Vector2.Dot(normal, normalNext);
                if (Mathf.Abs(dot) > 0.0001f)
                {
                    normal *= 1f / dot;
                }
            }

            Vector3 offset = (Vector3)(normal * _width * 0.5f);

            _vertices.Add(pathPoints[i] + offset);
            _vertices.Add(pathPoints[i] - offset);

            float u = (float)i / (pathPoints.Count - 1);
            _uvs.Add(new Vector2(u, 1f));
            _uvs.Add(new Vector2(u, 0f));
        }

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            int index = i * 2;

            _triangles.Add(index + 0);
            _triangles.Add(index + 2);
            _triangles.Add(index + 1);

            _triangles.Add(index + 2);
            _triangles.Add(index + 3);
            _triangles.Add(index + 1);
        }

        _mesh.SetVertices(_vertices);
        _mesh.SetUVs(0, _uvs);
        _mesh.SetTriangles(_triangles, 0);
        _mesh.RecalculateBounds();
    }

    private void ClearPath()
    {
        _mesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();
    }

    private Vector2 GetNormal(Vector2 dir)
    {
        return new Vector2(-dir.y, dir.x);
    }
}