using System;
using System.Collections;
using Unity.VisualScripting;
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
    private int _count = 0;

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

    #region Unity Lifecycle

    private void Start()
    {
        
    }
    private void Update()
    {
        CheckReflact(transform.position, Vector2.up);
        _lightMesh.DrawLine(_lightDomain.LightPath);
    }
    #endregion  
    private void CheckReflact(Vector3 origin, Vector3 direction)
    {
        _lightDomain.LightPath.Add(origin);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, _lightDomain.MaxDistance, _layerMask);
        if (hit.collider != null)
        {
            _lightDomain.LightPath.Add(hit.point);
            CheckReflact(hit.point + hit.normal * 0.1f, Vector2.Reflect(direction, hit.normal));
        }
        else
        {
            _lightDomain.LightPath.Add(origin + direction * _lightDomain.MaxDistance);
        }
    }

}
