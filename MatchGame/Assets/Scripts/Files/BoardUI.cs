using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField] private RectTransform shuffleButton;
    [SerializeField] private Sprite boardBackground;

    private SpriteRenderer _boardRenderer;

    public async UniTask CreateBoardBackground(int row, int column, float spacing, float cellSize)
    {
        if (boardBackground == null) return;

        float totalWidth = (column * cellSize) + ((column - 1) * spacing);
        float totalHeight = (row * cellSize) + ((row - 1) * spacing);


        if (_boardRenderer == null)
        {
            GameObject backgroundObject = new GameObject("BoardBackground");
            backgroundObject.transform.SetParent(transform);
            SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
            renderer.sprite = boardBackground;
            renderer.sortingOrder = -99;
            _boardRenderer = renderer;
        }
        

        _boardRenderer.gameObject.transform.localScale = new Vector3(
            (totalWidth / _boardRenderer.sprite.bounds.size.x) + 0.07f,
            (totalHeight / _boardRenderer.sprite.bounds.size.y) + 0.07f, 
            1);

        _boardRenderer.gameObject.transform.localPosition = Vector3.zero;
        await SetVisibilityBoardElements(true);
    }
    
    private Tween SetVisibilityShuffleButton(bool visibility)
    {
        return visibility
            ? Helpers.MoveOnScreen(shuffleButton.gameObject, Vector2.up, 1f)
            : Helpers.MoveOnScreen(shuffleButton.gameObject, Vector2.down, 1f);
    }

    private Tween SetVisibilityBoard(bool visibility)
    {
        return visibility
            ? Helpers.MoveOnScreen(transform.gameObject, Vector2.zero, 1f)
            : Helpers.MoveOnScreen(transform.gameObject, Vector2.right, 1f);
    }

    public Sequence SetVisibilityBoardElements(bool visibility)
    {
        return Sequence.Create().Group(SetVisibilityShuffleButton(visibility)).Group(SetVisibilityBoard(visibility));
    }
}
