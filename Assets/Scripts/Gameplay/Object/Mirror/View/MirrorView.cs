using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class MirrorView : MonoBehaviour, IPointerDownHandler, IDragHandler, IGetObjectType
{
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider2D;
    private MirrorDomain _mirrorDomain;
    private IMirrorMoveStrategy _moveStrategy;

    public void Install(MirrorDomain mirrorDomain, IMirrorMoveStrategy moveStrategy, Sprite sprite)
    {
        _mirrorDomain = mirrorDomain;
        _moveStrategy = moveStrategy;
        _spriteRenderer.sprite = sprite;

        transform.position = mirrorDomain.Position;
        transform.rotation = mirrorDomain.Rotation;

        Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;
        _boxCollider2D.size = spriteSize;

        gameObject.SetActive(true);
    }

    #region Unity Lifecycle
    public void Awake()
    {
        gameObject.layer = 6;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
    public void OnDisable()
    {
        _mirrorDomain = null;
        _moveStrategy = null;
        _spriteRenderer.sprite = null;
        gameObject.SetActive(false);
    }
    #endregion

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);

        _moveStrategy.Move(transform, touchPosition);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public ObjectType GetObjectType()
    {
        return _mirrorDomain.MirrorType;
    }
}