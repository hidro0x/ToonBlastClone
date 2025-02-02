using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [Header("Board Settings")] 
    [Tooltip("The margin of the table to be formed from the right and left axis")] [SerializeField]
    float margin = 0.1f;
    
    [Tooltip("Margin between cells")] [SerializeField]
    float spacing = 0.1f;

    [Space]
    [Header("Board Assets")]
    [SerializeField] private RectTransform shuffleButton;
    [SerializeField] private Sprite boardBackground;
    
    private float _fixedSpacing;
    private float _fixedMargin;
    private Camera _camera;
    private SpriteRenderer _boardRenderer;
    private void Awake()
    {
        _camera = Camera.main;
    }

    public async UniTask CreateBoard(Board board)
    {
        int row = board.BoardData.GetLength(0);
        int column = board.BoardData.GetLength(1);

        int rowForSizing = row < 5 ? 5 : row;
        int columnForSizing = column < 5 ? 5 : column;
        
        float height = 2f * _camera.orthographicSize;
        float width = height * _camera.aspect;
        _fixedSpacing = (_camera.aspect / -6.5f) * spacing;
        _fixedMargin = margin * (_camera.aspect / 6.5f);

        float maxCellWidth = (width - 2 * _fixedMargin - (columnForSizing - 1) * _fixedSpacing) / columnForSizing;
        float maxCellHeight = (height - 2 * _fixedMargin - (rowForSizing - 1) * _fixedSpacing) / rowForSizing;

        float cellSize = Mathf.Min(maxCellWidth, maxCellHeight);

        Vector3 cellScale = new Vector3(cellSize * 1.15f, cellSize * 1.15f, 1);

        await board.BoardPool.InitializePoolsAsync(board, cellScale);
        BlockManager.Instance.SetBlockSize(0.5f);
        

        Vector3 startPosition = new Vector3(-((column - 1) * (cellSize + _fixedSpacing)) / 2,
            ((row - 1) * (cellSize + _fixedSpacing)) / 2, 0);
        
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Vector3 position = startPosition + new Vector3(j * (cellSize + _fixedSpacing),
                    -i * (cellSize + _fixedSpacing), 0);
                
                board.BoardData[i, j] = board.BoardPool.TilePool.Get();
                board.BoardData[i, j].transform.localPosition = position;
            }
        }
        
        CreateBoardBackground(row, column, _fixedSpacing, cellSize);
    }
    
    private void CreateBoardBackground(int row, int column, float spacing, float cellSize)
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
