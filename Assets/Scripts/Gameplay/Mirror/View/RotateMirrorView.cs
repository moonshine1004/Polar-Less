using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class RotateMirrorView : MirrorBaseView
{
    [SerializeField] private Image _pressedUI;

    private void Awake()
    {
        gameObject.layer = 6;
        _mirrorType = MirrorType.Rotate;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // UI 표시
        // SwithControl();
        _isCotrolling = true;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if( !_isCotrolling) return;
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
        _isCotrolling = !_isCotrolling;
    }

}