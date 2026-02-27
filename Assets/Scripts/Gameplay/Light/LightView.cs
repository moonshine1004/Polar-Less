using UnityEngine;
using GamePlay.Light.UseCase;
using GamePlay.Light.Domain;
using UnityEngine.InputSystem.iOS;

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
    private LightReflectUseCase _lightReflectUseCase = new LightReflectUseCase();


    public void InstallLightView(LightDomain lightDomain, LightStartUseCase lightStartUseCase)
    {
        _lightDomain = lightDomain;
        _lightStartUseCase = lightStartUseCase;
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
    }
    private void Update()
    {
        //ReflectLaser(transform.position, transform.up);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _lightDomain.MaxDistance, _layerMask);
        if (hit.collider != null)
        {
            DrawLightPath(_lightReflectUseCase.Excute(this, _lightDomain, hit.point, hit.normal));
        }
    }
    #endregion  
    private void ReflectLaser(Vector2 origin, Vector2 direction)
    {
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, origin);

        int reflections = 0;
        Vector2 currentPosition = origin;
        Vector2 currentDirection = direction;

        while (reflections < _lightDomain.MaxReflections)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentDirection, _lightDomain.MaxDistance, _layerMask);
            if (hit.collider != null)
            {
                reflections++;
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(reflections, hit.point);

                // 반사 방향 계산
                currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                currentPosition = hit.point;

                // 목표 지점에 도달 시 체크 (Layer로 Goal 처리 가능)
                if (hit.collider.CompareTag("Goal"))
                {
                    Debug.Log("Goal Reached!");
                    break;
                }
            }
            else
            {
                // 더 이상 충돌 없음 → 끝까지 직선
                _lineRenderer.positionCount += 1;
                _lineRenderer.SetPosition(reflections + 1, currentPosition + currentDirection * _lightDomain.MaxDistance);
                break;
            }
        }
    } 

    private void DrawLightPath(LightDrawData lightDrawData)
    {
        _lineRenderer.SetPosition(lightDrawData.Index, lightDrawData.Origin);
        _lineRenderer.SetPosition(lightDrawData.Index + 1, lightDrawData.Origin + lightDrawData.Direction.normalized * lightDrawData.MaxDistance);
    }


    
}
