using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class RotateMirrorView : MirrorBaseView
{
    
    private bool _isControlling = false;

    private IEventBusSubscription _eventBusSubscription;
    private IEventBusPublish _eventBusPublish;

    private void Awake()
    {
        gameObject.layer = 6;
        _mirrorDomain = new MirrorDomain()
        {
            MirrorType = MirrorType.Rotate,
            MirrorID = 1,
        };
        _collider = GetComponent<Collider2D>();

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // UI 표시
        // SwithControl();
        _isControlling = true;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if( !_isControlling) return;
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2 mirrorPosition = transform.position;

        if(touchPosition.x < mirrorPosition.x)
        {
            Vector2 direction = touchPosition - mirrorPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            Vector2 direction = mirrorPosition - touchPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    

    protected override void Move()
    {
        
    }

    public void SwithControl()
    {
        _isControlling = !_isControlling;
    }

}