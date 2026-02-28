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
    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer => _lineRenderer;
    private LayerMask _layerMask = 1 << 6;
    private int _count = 0;

    private LightDomain _lightDomain;
    private LightStartUseCase _lightStartUseCase;
    private LightReflectUseCase _lightReflectUseCase;

    // private event Action MirrorTouched;

    public void InstallLightView(LightDomain lightDomain, LightServices lightServices)
    {
        _lightDomain = lightDomain;
        _lightStartUseCase = lightServices.Get<LightStartUseCase>();
        _lightReflectUseCase = lightServices.Get<LightReflectUseCase>();
    }

    #region Unity Lifecycle
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
    }
    private void Start()
    {
        DrawLightPath(_lightStartUseCase.Excute(this, _lightDomain));
    }
    private void Update()
    {
        _count = 0;
        DrawLightPath(_lightStartUseCase.Excute(this, _lightDomain));
        DrawLine(transform.position, Vector3.up);
    }
    #endregion  
    private void DrawLine(Vector3 origin, Vector3 direction)
    {
        if(_count >= _lightDomain.MaxReflections) return;
        _count++;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, _lightDomain.MaxDistance, _layerMask);
        if (hit.collider != null)
        {
            var data = _lightReflectUseCase.Excute(this, _lightDomain,hit.point ,direction, hit.normal);
            DrawLightPath(data);
            DrawLine(data.Origin, data.Direction);
        }
    }

    private void DrawLightPath(LightDrawData lightDrawData)
    {
        _lineRenderer.SetPosition(lightDrawData.Index, lightDrawData.Origin);
        _lineRenderer.SetPosition(lightDrawData.Index + 1, lightDrawData.Origin + lightDrawData.Direction.normalized * lightDrawData.MaxDistance);
    }
}
