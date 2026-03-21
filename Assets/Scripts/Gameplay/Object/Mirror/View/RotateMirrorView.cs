using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class RotateMirrorView : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    
    private bool _isControlling = false;
    private MirrorDomain _mirrorDomain = new MirrorDomain()
    {
        MirrorType = MirrorType.Rotate
    };
    private Collider2D _collider;
    private IMirrorMoveStrategy _moveStrategy;

    

    private void Awake()
    {
        gameObject.layer = 6;
        _collider = GetComponent<Collider2D>();
        _moveStrategy = new RotateMirrorMoveStrategy();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // UI 표시
        // SwithControl();
        _isControlling = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if( !_isControlling) return;
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);

        _moveStrategy.Move(transform, touchPosition);
    }
    



    public void SwithControl()
    {
        _isControlling = !_isControlling;
    }

}