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

        if (pathPoints == null || pathPoints.Count < 2)
            return;

        float halfWidth = _width * 0.5f;

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 start = transform.InverseTransformPoint(pathPoints[i]);
            Vector3 end = transform.InverseTransformPoint(pathPoints[i + 1]);

            Vector2 dir = end - start;
            float len = dir.magnitude;

            if (len < 0.0001f)
                continue;

            dir /= len;

            Vector2 normal = new Vector2(-dir.y, dir.x) * halfWidth;

            Vector3 v0 = start + (Vector3)normal;
            Vector3 v1 = start - (Vector3)normal;
            Vector3 v2 = end + (Vector3)normal;
            Vector3 v3 = end - (Vector3)normal;

            int index = _vertices.Count;

            _vertices.Add(v0);
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);

            _uvs.Add(new Vector2(0f, 1f));
            _uvs.Add(new Vector2(0f, 0f));
            _uvs.Add(new Vector2(1f, 1f));
            _uvs.Add(new Vector2(1f, 0f));

            _triangles.Add(index + 0);
            _triangles.Add(index + 2);
            _triangles.Add(index + 1);

            _triangles.Add(index + 2);
            _triangles.Add(index + 3);
            _triangles.Add(index + 1);

            if (i < pathPoints.Count - 2)
            {
                Vector3 next = pathPoints[i + 2];

                Vector2 nextDir = (next - end).normalized;
                Vector2 nextNormal = new Vector2(-nextDir.y, nextDir.x) * halfWidth;

                Vector3 joinA = end + (Vector3)normal;
                Vector3 joinB = end + (Vector3)nextNormal;

                int joinIndex = _vertices.Count;

                _vertices.Add(end);
                _vertices.Add(joinA);
                _vertices.Add(joinB);

                _uvs.Add(new Vector2(0.5f, 0.5f));
                _uvs.Add(new Vector2(0f, 1f));
                _uvs.Add(new Vector2(1f, 1f));

                _triangles.Add(joinIndex + 0);
                _triangles.Add(joinIndex + 1);
                _triangles.Add(joinIndex + 2);
            }
        }

        if (_vertices.Count == 0)
            return;

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
}