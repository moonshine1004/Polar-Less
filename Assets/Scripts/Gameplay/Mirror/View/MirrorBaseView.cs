using UnityEngine;
using UnityEngine.EventSystems;



public abstract class MirrorBaseView : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Collider2D _collider;
    protected MirrorDomain _mirrorDomain;

    public void InstallMirror(MirrorDomain mirrorDomain)
    {
        _mirrorDomain = mirrorDomain;
    }

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
