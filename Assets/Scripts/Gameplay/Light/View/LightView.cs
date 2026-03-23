using UnityEngine;

public class LightView : MonoBehaviour
{
    private LayerMask _layerMask = 1 << 6;

    [SerializeField] private LightMesh _lightMesh;

    private LightDomain _lightDomain;

    public void InstallLightView(LightDomain lightDomain, LightServices lightServices)
    {
        _lightDomain = lightDomain;
    }

    private void Update()
    {
        _lightDomain.LightPath.Clear();
        _lightDomain.LightPath.Add(transform.position);

        CheckReflect(transform.position, Vector2.up);

        _lightMesh.DrawLine(_lightDomain.LightPath);
    }

    private void CheckReflect(Vector3 origin, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, _lightDomain.MaxDistance, _layerMask);
        var objectType = hit.collider?.GetComponent<IGetObjectType>()?.GetObjectType();
        if (objectType is ObjectType.Rotate or ObjectType.Move or ObjectType.Static)
        {
            _lightDomain.LightPath.Add(hit.point);

            CheckReflect(hit.point + hit.normal * 0.001f, Vector2.Reflect(direction.normalized, hit.normal).normalized);
        }
        else if (objectType == ObjectType.Obstacle)
        {
            _lightDomain.LightPath.Add(hit.point);
        }
        else if (objectType == ObjectType.Goal)
        {
            _lightDomain.LightPath.Add(hit.point);
            Debug.Log("Goal Reached!");
        }
        else
        {
            _lightDomain.LightPath.Add(origin + direction.normalized * _lightDomain.MaxDistance);
        }
    }
}