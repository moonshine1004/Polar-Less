using UnityEngine;
using UnityEngine.EventSystems;

public enum MirrorType
{
    Rotate,
    Move,
    Static
}

public abstract class MirrorBaseView : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    protected LayerMask _layerMask = 1 << 6;
    private Collider2D _collider;
    [SerializeField] protected int _mirrorID;
    [SerializeField] protected MirrorType _mirrorType;
    [SerializeField] protected bool _isCotrolling = false;

    public int MirrorID{ get => _mirrorID; set => _mirrorID = value; }


    #region Unity Lifecycle
    private void Awake()
    {
        gameObject.layer = 6;
        _collider = GetComponent<Collider2D>();
    }
    #endregion
    public abstract void OnPointerDown(PointerEventData eventData);
    public abstract void OnDrag(PointerEventData eventData);
    protected abstract void Move();

}
