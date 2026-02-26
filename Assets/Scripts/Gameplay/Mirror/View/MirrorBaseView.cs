using UnityEngine;

[ExecuteAlways]
public class MirrorBaseView : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask = 1 << 6;
    private Collider2D _collider;
    private LineRenderer _lineRenderer;

    #region Unity Lifecycle
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    #endregion

    protected virtual void Move()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        _lineRenderer = collision.gameObject.GetComponent<LineRenderer>();
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount, transform.position + new Vector3(3, 3, 0));
    }
    private void Reflect()
    {
        
    }
}
