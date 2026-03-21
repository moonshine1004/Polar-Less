using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class SlideMirrorView : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private bool _isControlling = false;
    private MirrorDomain _mirrorDomain = new MirrorDomain()
    {
        MirrorType = MirrorType.Move
    };
    private Collider2D _collider;

    private void Awake()
    {
        gameObject.layer = 6;
        _collider = GetComponent<Collider2D>();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}