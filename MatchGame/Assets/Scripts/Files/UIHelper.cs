using UnityEngine;
using PrimeTween;

public static class UIHelper
{
    public static Tween MoveOnScreen(GameObject gameObject, Vector2 direction, float duration = 1.5f)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        return rectTransform != null ? MoveUIOnScreen(rectTransform, direction, duration) : MoveGameObjectOnScreen(gameObject.transform, 10, 10, direction, duration);
    }
    
    
    private static Tween MoveUIOnScreen(RectTransform uiElement, Vector2 direction, float duration)
    {
        Vector2 targetPosition = new Vector2();
        if (direction.y != 0)
        {
            targetPosition = direction == Vector2.up ? new Vector2(0, Mathf.Abs(uiElement.anchoredPosition.y)) : new Vector2(0, -uiElement.anchoredPosition.y);
        }
        else if(direction.x != 0)
        {
            targetPosition = direction == Vector2.right ? new Vector2(Mathf.Abs(uiElement.anchoredPosition.x), 0) : new Vector2(-uiElement.anchoredPosition.x, 0);
        }


        return Tween.UIAnchoredPosition(uiElement, targetPosition, duration, Ease.InOutQuad);
    }
    
    private static Tween MoveGameObjectOnScreen(Transform transform, float width, float height, Vector2 direction, float duration)
    {
        if(direction == Vector2.zero) return Tween.LocalPosition(transform,Vector3.zero, duration,Ease.InOutQuad);
        
        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        
        float objectSize = direction.x != 0 ? width : height;
        
        Vector3 targetPosition = transform.position + (Vector3)direction * (screenBounds.x + objectSize);
        
        return Tween.LocalPosition(transform,targetPosition, duration,Ease.InOutQuad);
    }
    
}