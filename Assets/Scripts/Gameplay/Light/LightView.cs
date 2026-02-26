using UnityEngine;

public class LightView : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private LayerMask _layerMask = 1 << 6;
    private int _maxReflections = 100;
    private float _maxDistance = 18f;
    [SerializeField] private int _positionIndex = 1; // 마지막 점
    [SerializeField] private GameObject _lastMirror;
    private Vector2 _currentPosition; // 레이저의 최근 출발 지점
    private Vector2 _currentDirection; // 레이저의 방향

    #region Unity Lifecycle
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _currentPosition = transform.position;
        _currentDirection = Vector2.up;
        _lastMirror = this.gameObject;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _currentPosition);
        _lineRenderer.SetPosition(1, _currentPosition + _currentDirection * _maxDistance);
        
    }
    private void Update()
    {
        Reflect(_currentPosition, _currentDirection);
    }
    #endregion   

    /// <summary>
    /// </summary>
    /// <param name="currentPosition">마지막 출발지점</param>
    /// <param name="currentDirection">입사 방향</param>
    private void Reflect(Vector2 currentPosition, Vector2 currentDirection)
    {
        if(_maxReflections <= 0) return;
        _maxDistance--;
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentDirection, _maxDistance, _layerMask);
        if (hit.collider != null)
        {
            if(_lastMirror != hit.collider.gameObject) // 새 거울 -> 점 추가 
            {
                _lastMirror = hit.collider.gameObject;
                
                _lineRenderer.positionCount += 1;

                _currentPosition = hit.point;
                _lineRenderer.SetPosition(_positionIndex, _currentPosition);
                _currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                _lineRenderer.SetPosition(_positionIndex++ + 1, _currentPosition + _currentDirection * _maxDistance);

            }
            else if(_lastMirror == hit.collider.gameObject) // 기존 거울 -> 기존 점 수정 및 다음 점 수정
            {
                Debug.Log("Same mirror hit again: " );
                _currentPosition = hit.point;
                _lineRenderer.SetPosition(_positionIndex - 1, _currentPosition);
                _currentDirection = Vector2.Reflect(currentDirection, hit.normal);
                _lineRenderer.SetPosition(_positionIndex, _currentPosition + _currentDirection * _maxDistance);
            }
            Reflect(_currentPosition, _currentDirection);
        }
    }


    
}
