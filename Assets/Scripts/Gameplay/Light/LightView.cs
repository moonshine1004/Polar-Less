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
    }
    private void Start()
    {
        DrawLightPath(_lightStartUseCase.Excute(this, _lightDomain));
        //StartCoroutine(OnStartDraw());
    }
    private void Update()
    {
        DrawLine(transform.position, Vector3.up);
    }
    #endregion  
    private void DrawLine(Vector3 origin, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, _lightDomain.MaxDistance, _layerMask);
        if (hit.collider != null)
        {
            var data = _lightReflectUseCase.Excute(this, _lightDomain, hit.point, hit.normal);
            DrawLightPath(data);
            DrawLine(data.Origin, data.Direction);
        }
    }
    
    private IEnumerator OnStartDraw()
    {
        bool hitMirror = true;
        while (hitMirror)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _lightDomain.MaxDistance, _layerMask);
            if (hit.collider != null)
            {
                var data = _lightReflectUseCase.Excute(this, _lightDomain, hit.point, hit.normal);
                DrawLightPath(data);
                RaycastHit2D mirrorHit = Physics2D.Raycast(data.Origin, data.Direction, data.MaxDistance, _layerMask);
                if (mirrorHit.collider != null)
                {
                    var data2 = _lightReflectUseCase.Excute(this, _lightDomain, mirrorHit.point, mirrorHit.normal);
                    DrawLightPath(data2);
                }
                hitMirror = false;
                yield return null;
            }
            yield return null;  
        }
    }

    private void DrawLightPath(LightDrawData lightDrawData)
    {
        _lineRenderer.SetPosition(lightDrawData.Index, lightDrawData.Origin);
        _lineRenderer.SetPosition(lightDrawData.Index + 1, lightDrawData.Origin + lightDrawData.Direction.normalized * lightDrawData.MaxDistance);
    }
}
