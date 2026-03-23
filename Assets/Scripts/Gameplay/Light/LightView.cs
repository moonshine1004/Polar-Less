using UnityEngine;

public struct LightDrawData
{
    public int Index;
    public Vector2 Origin;
    public Vector2 Direction;
    public float MaxDistance;
}

public class LightView : MonoBehaviour
{
    private LayerMask _layerMask = 1 << 6;

    [SerializeField] private LightMesh _lightMesh;

    private LightDomain _lightDomain;
    private LightStartUseCase _lightStartUseCase;
    private LightReflectUseCase _lightReflectUseCase;

    public void InstallLightView(LightDomain lightDomain, LightServices lightServices)
    {
        _lightDomain = lightDomain;
        _lightStartUseCase = lightServices.Get<LightStartUseCase>();
        _lightReflectUseCase = lightServices.Get<LightReflectUseCase>();
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

        if (hit.collider != null)
        {
            _lightDomain.LightPath.Add(hit.point);

            CheckReflect(hit.point + hit.normal * 0.001f, Vector2.Reflect(direction.normalized, hit.normal).normalized);
        }
        else
        {
            _lightDomain.LightPath.Add(origin + direction.normalized * _lightDomain.MaxDistance);
        }
    }
}