using UnityEngine;
using UnityEngine.EventSystems;



public class MirrorView : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private MirrorDomain _mirrorDomain = new MirrorDomain();
    [SerializeField] private MirrorType _mirrorType;
    private IMirrorMoveStrategy _moveStrategy;

    private void Awake()
    {
        switch (_mirrorType)
        {
            case MirrorType.Rotate:
                _moveStrategy = new RotateMirrorMoveStrategy();
                break;
            case MirrorType.Move:
                _moveStrategy = new SlideMirrorMoveStrategy();
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);

        _moveStrategy.Move(transform, touchPosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}