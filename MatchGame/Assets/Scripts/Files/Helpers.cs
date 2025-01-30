using System;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Helpers
{
    public static Tween MoveOnScreen(GameObject gameObject, Vector2 direction, float duration = 1.5f)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        return rectTransform != null
            ? MoveUIOnScreen(rectTransform, direction, duration)
            : MoveGameObjectOnScreen(gameObject.transform, 10, 10, direction, duration);
    }


    private static Tween MoveUIOnScreen(RectTransform uiElement, Vector2 direction, float duration)
    {
        Vector2 targetPosition = new Vector2();
        if (direction.y != 0)
        {
            targetPosition = direction == Vector2.up
                ? new Vector2(0, Mathf.Abs(uiElement.anchoredPosition.y))
                : new Vector2(0, -uiElement.anchoredPosition.y);
        }
        else if (direction.x != 0)
        {
            targetPosition = direction == Vector2.right
                ? new Vector2(Mathf.Abs(uiElement.anchoredPosition.x), 0)
                : new Vector2(-uiElement.anchoredPosition.x, 0);
        }


        return Tween.UIAnchoredPosition(uiElement, targetPosition, duration, Ease.InOutQuad);
    }

    private static Tween MoveGameObjectOnScreen(Transform transform, float width, float height, Vector2 direction,
        float duration)
    {
        if (direction == Vector2.zero) return Tween.LocalPosition(transform, Vector3.zero, duration, Ease.InOutQuad);

        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float objectSize = direction.x != 0 ? width : height;

        Vector3 targetPosition = transform.position + (Vector3)direction * (screenBounds.x + objectSize);

        return Tween.LocalPosition(transform, targetPosition, duration, Ease.InOutQuad);
    }
    
    public static List<Vector2Int> SelectRandomElements(int row, int column, int x)
    {
        List<Vector2Int> allElements = new List<Vector2Int>();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                allElements.Add(new Vector2Int(i,j));
            }
        }

        List<Vector2Int> selected = new List<Vector2Int>();

        for (int i = 0; i < x; i++)
        {
            if (allElements.Count == 0) break; 
            int randomIndex = Random.Range(0, allElements.Count);
            selected.Add(allElements[randomIndex]);
            allElements.RemoveAt(randomIndex);
        }

        return selected;
    }
    
    public static List<int> GenerateRandomDivisors(int x)
    {
        System.Random random = new System.Random();
        List<int> numbers = new List<int>();

        while (x >= 4) 
        {
            int part = random.Next(2, x - 1); 
            numbers.Add(part);
            x -= part;
        }

        numbers.Add(x); 
        return numbers;
    }

}