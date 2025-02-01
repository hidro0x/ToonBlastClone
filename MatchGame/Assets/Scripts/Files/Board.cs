using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using Random = System.Random;

public class Board : MonoBehaviour
{
    [Header("Board Settings")] [SerializeField]
    private LevelData level;

    private int _rowsLength;
    private int _columnsLength;

    [Tooltip("Margin between cells")] [SerializeField]
    float spacing = 0.1f;

    private float _fixedSpacing;
    private float _fixedMargin;

    [Tooltip("The margin of the table to be formed from the right and left axis")] [SerializeField]
    float margin = 0.1f;

    [Header("Assets")] [SerializeField] private RectTransform shuffleButton;
    [SerializeField] private Sprite boardBackground;

    public Tile[,] BoardData { get; private set; }
    private Camera _camera;
    public ObjectPool<Block> BlockPool { get; private set; }
    public ObjectPool<Tile> TilePool { get; private set; }
    
    private readonly List<Tile> _matchedTiles = new();
    private readonly Stack<int> _tempStack = new();
    private bool[] _visitedCells;
    
    private bool _canShuffle = true;

    #region MonoBehaviour

    async void Start()
    {
        _camera = Camera.main;
        
        await CreateBoard();
    }

    private void OnEnable()
    {
        InputHandler.OnTileClicked += CheckTile;
    }

    private void OnDisable()
    {
        InputHandler.OnTileClicked -= CheckTile;
    }

    #endregion

    #region Shuffling

    public async void StartShuffle()
    {
        if (!_canShuffle) return;
        
        for (var i = 0; i < _rowsLength; i++)
        for (var j = 0; j < _columnsLength; j++)
        {
            if (BoardData[i,j].IsTileFilled)
            {
                BoardData[i,j].Block.CompleteAnimation();
            }
        }

        InputHandler.OnControlInput?.Invoke(false);
        _canShuffle = false;

        await SetVisibilityBoardElements(false);

        ShuffleBoard();

        await SetVisibilityBoardElements(true);

        CheckBlockGroups();

        InputHandler.OnControlInput?.Invoke(true);
        _canShuffle = true;
    }

    
    private void ShuffleBoard()
    {
        Random random = new Random();

        List<Block> tempBlocksList = new List<Block>();
        for (int i = 0; i < _rowsLength; i++)
        {
            for (int j = 0; j < _columnsLength; j++)
            {
                tempBlocksList.Add(BoardData[i, j].Block);
            }
        }

        for (int i = tempBlocksList.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (tempBlocksList[i], tempBlocksList[j]) = (tempBlocksList[j], tempBlocksList[i]);
        }

        int index = 0;
        for (int i = 0; i < _rowsLength; i++)
        {
            for (int j = 0; j < _columnsLength; j++)
            {
                BoardData[i, j].MarkAsEmpty();
                BoardData[i, j].AssignBlock(tempBlocksList[index++], true);
            }
        }

        HandleDeadlock();
    }

    private void HandleDeadlock()
    {
        Random random = new Random();
        var floodCounts = Helpers.GenerateRandomDivisors(random.Next(2, Math.Max(_rowsLength, _columnsLength)));
        var randomTiles = Helpers.SelectRandomElements(_rowsLength, _columnsLength, floodCounts.Count);
        

        for (int i = 0; i < floodCounts.Count; i++)
        {
            FillTilesWithAmount(BoardData[randomTiles[i].x, randomTiles[i].y], floodCounts[i]);
        }
    }

    private void FillTilesWithAmount(Tile startTile, int count)
    {
        SetFloodFillCache();
        var floodCount = count;

        int startRow = startTile.Row;
        int startColumn = startTile.Column;
        
        _tempStack.Push(GetIndex(startRow, startColumn, _columnsLength));
        while (floodCount > 0)
        {
            int index = _tempStack.Pop();
            int row = index / _columnsLength;
            int column = index % _columnsLength;

            if (IsOutOfBounds(row, column) || _visitedCells[index])
                continue;

            Tile currentTile = BoardData[row, column];
            if (!currentTile.IsTileFilled) continue;
            currentTile.Block.ChangeBlock(startTile.Block.Data);
            floodCount--;
            _visitedCells[index] = true;

            AddTilesToStack(row, column, _tempStack);
        }
        
    }

    

    #endregion

    #region Checks
    
    private bool IsBoardPlayable()
    {
        for (var i = 0; i < _rowsLength; i++)
        for (var j = 0; j < _columnsLength; j++)
        {
            var tile = BoardData[i, j];
            if (!tile.IsTileFilled)
                continue;

            if (HasMatchingNeighbors(tile))
                return true;
        }

        return false;
    }

    private bool HasMatchingNeighbors(Tile tile)
    {
        var directions = new (int rowOffset, int colOffset)[]
        {
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1)
        };

        BlockColor targetColor = tile.Block.Data.BlockColor;

        foreach (var (rowOffset, colOffset) in directions)
        {
            int newRow = tile.Row + rowOffset;
            int newColumn = tile.Column + colOffset;

            if (!IsOutOfBounds(newRow, newColumn))
            {
                Tile neighbor = BoardData[newRow, newColumn];
                if (neighbor.IsTileFilled && neighbor.Block.Data.BlockColor == targetColor)
                    return true;
            }
        }

        return false;
    }

    private void CheckTile(Tile tile)
    {
        if (tile.Block != null)
        {
            var matchedTiles = GetTilesSameColor(tile);
            if (matchedTiles.Count < 2)
            {
                BlockManager.Instance.ShakeBlock(tile.Block);
                return;
            }

            foreach (var element in matchedTiles)
            {
                element.RemoveBlock();
            }

            CheckRows(matchedTiles);
        }
    }

    private void CheckRows(List<Tile> tiles)
    {
        var fillingColumns = new List<int>();
        foreach (var tile in tiles)
        {
            if (fillingColumns.Contains(tile.Column)) continue;
            fillingColumns.Add(tile.Column);
        }

        FillColumns(fillingColumns);
    }

    private void CheckBlockGroups()
    {
        for (var j = 0; j < _columnsLength; j++)
        {
            for (var i = 0; i < _rowsLength; i++)
            {
                if (IsOutOfBounds(i, j)) continue;
                var matchedTiles = GetTilesSameColor(BoardData[i, j]);
                foreach (var tile in matchedTiles)
                {
                    BlockManager.Instance.SetBlockType(tile.Block, matchedTiles.Count);
                }
            }
        }
    }

    #endregion

    #region Filling

    private void OrderColumn(int columnNum)
    {
        for (int row = _rowsLength - 1; row >= 0; row--)
        {
            Tile currentTile = BoardData[row, columnNum];
            if (currentTile.IsTileFilled) continue;

            for (int upperRow = row - 1; upperRow >= 0; upperRow--)
            {
                Tile upperTile = BoardData[upperRow, columnNum];

                if (!upperTile.IsTileFilled) continue;
                BlockManager.Instance.MoveBlock(upperTile, currentTile);
                break;
            }
        }
    }
    
    private async UniTask InitializePoolsAsync(int amount, Vector3 cellSize)
    {
        if (BlockPool == null)
        {
            var tempBlockSpriteObject = new GameObject().AddComponent<Block>();
            tempBlockSpriteObject.gameObject.AddComponent<SpriteRenderer>();
            tempBlockSpriteObject.transform.localScale = cellSize;
            BlockPool = new ObjectPool<Block>(tempBlockSpriteObject, _rowsLength * _columnsLength, transform);
            tempBlockSpriteObject.gameObject.SetActive(false);
        }

        if (TilePool == null)
        {
            var tempTileObject = new GameObject().AddComponent<Tile>();
            tempTileObject.gameObject.AddComponent<BoxCollider2D>();
            tempTileObject.transform.localScale = cellSize;
            TilePool = new ObjectPool<Tile>(tempTileObject, _rowsLength * _columnsLength, transform);
            tempTileObject.gameObject.SetActive(false);
        }
        
        await BlockPool.EnsurePoolSizeAsync(amount);
        await TilePool.EnsurePoolSizeAsync(amount);
        
        var blockObjects = BlockPool.GetAllObjectsTransforms();
        var tileObjects = TilePool.GetAllObjectsTransforms();
        
        await UniTask.WhenAll(
            SetScaleAsync(blockObjects, cellSize), 
            SetScaleAsync(tileObjects, cellSize) 
        );
    }
    
    

    private void FillColumns(List<int> columns)
    {
        foreach (var column in columns)
        {
            OrderColumn(column);
            BlockManager.Instance.SpawnBlock(column);
        }

        CheckBlockGroups();

        if (IsBoardPlayable()) return;
        StartShuffle();
    }

    private async UniTask CreateBoard(LevelData data = null)
    {
        _rowsLength = data == null ? 10 : level.Row;
        _columnsLength = data == null ? 9 : level.Column;
        
        
        BoardData = new Tile[_rowsLength, _columnsLength];

        float height = 2f * _camera.orthographicSize;
        float width = height * _camera.aspect;
        _fixedSpacing = (_camera.aspect / -6.5f) * spacing;
        _fixedMargin = margin * (_camera.aspect / 6.5f);

        float maxCellWidth = (width - 2 * _fixedMargin - (_columnsLength - 1) * _fixedSpacing) / _columnsLength;
        float maxCellHeight = (height - 2 * _fixedMargin - (_rowsLength - 1) * _fixedSpacing) / _rowsLength;

        float cellSize = Mathf.Min(maxCellWidth, maxCellHeight);

        Vector3 cellScale = new Vector3(cellSize * 1.15f, cellSize * 1.15f, 1);

        await InitializePoolsAsync(_rowsLength * _columnsLength, cellScale);
        BlockManager.Instance.SetBlockSize(0.5f);
        

        Vector3 startPosition = new Vector3(-((_columnsLength - 1) * (cellSize + _fixedSpacing)) / 2,
            ((_rowsLength - 1) * (cellSize + _fixedSpacing)) / 2, 0);
        
        for (int i = 0; i < _rowsLength; i++)
        {
            for (int j = 0; j < _columnsLength; j++)
            {
                Vector3 position = startPosition + new Vector3(j * (cellSize + _fixedSpacing),
                    -i * (cellSize + _fixedSpacing), 0);
                
                BoardData[i, j] = TilePool.Get();
                BoardData[i, j].transform.localPosition = position;
                
                BoardData[i, j].Init(i, j,
                    level == null
                        ? BlockManager.Instance.GetRandomBlock()
                        : BlockManager.Instance.GetBlock(level.Board[j, i].BlockColor));
            }
        }

        if (!IsBoardPlayable())
        {
            HandleDeadlock();
        }
        
        CheckBlockGroups();
        await CreateBoardBackground(cellSize);
    }

    private async UniTask CreateBoardBackground(float cellSize)
    {
        if (boardBackground == null) return;

        float totalWidth = (_columnsLength * cellSize) + ((_columnsLength - 1) * _fixedSpacing);
        float totalHeight = (_rowsLength * cellSize) + ((_rowsLength - 1) * _fixedSpacing);

        GameObject backgroundObject = new GameObject("BoardBackground");
        backgroundObject.transform.SetParent(transform);

        SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
        renderer.sprite = boardBackground;
        renderer.sortingOrder = -99;

        backgroundObject.transform.localScale = new Vector3(
            (totalWidth / renderer.sprite.bounds.size.x) + 0.07f,
            (totalHeight / renderer.sprite.bounds.size.y) + 0.07f, 
            1);

        backgroundObject.transform.localPosition = Vector3.zero;
        await SetVisibilityBoardElements(true);
    }

    private List<Tile> GetTilesSameColor(Tile startTile)
    {
        SetFloodFillCache();
        
        int startRow = startTile.Row;
        int startColumn = startTile.Column;
        BlockColor targetColor = startTile.Block.Data.BlockColor;

        _tempStack.Push(GetIndex(startRow, startColumn, _columnsLength));

        _matchedTiles.Clear();

        while (_tempStack.Count > 0)
        {
            int index = _tempStack.Pop();
            int row = index / _columnsLength;
            int column = index % _columnsLength;

            if (IsOutOfBounds(row, column) || _visitedCells[index])
                continue;

            Tile currentTile = BoardData[row, column];
            if (!currentTile.IsTileFilled || currentTile.Block.Data.BlockColor != targetColor) continue;

            _visitedCells[index] = true;
            _matchedTiles.Add(currentTile);

            AddTilesToStack(row, column, _tempStack);
        }

        return _matchedTiles;
    }
    
    private void AddTilesToStack(int row, int column, Stack<int> stack)
    {
        var directions = new (int rowOffset, int colOffset)[]
        {
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1)
        };

        foreach (var (rowOffset, colOffset) in directions)
        {
            int newRow = row + rowOffset;
            int newColumn = column + colOffset;

            if (!IsOutOfBounds(newRow, newColumn))
            {
                stack.Push(GetIndex(newRow, newColumn, _columnsLength));
            }
        }
    }
    
    
    #endregion

    #region Helpers
    
    private void SetFloodFillCache()
    {
        int totalCells = _rowsLength * _columnsLength;

        if (_visitedCells == null || _visitedCells.Length < totalCells)
            _visitedCells = new bool[totalCells];
        else
            Array.Clear(_visitedCells, 0, totalCells);
    }

    private bool IsOutOfBounds(int row, int column)
    {
        return row < 0 || row >= _rowsLength || column < 0 || column >= _columnsLength;
    }

    private int GetIndex(int row, int column, int columnCount)
    {
        return row * columnCount + column;
    }

    public int GetEmptyTileCountOnColumn(int columnNum)
    {
        int emptyTileAmount = 0;
        for (int row = _rowsLength - 1; row >= 0; row--)
        {
            Tile currentTile = BoardData[row, columnNum];
            if (currentTile.IsTileFilled) continue;
            emptyTileAmount++;
        }

        return emptyTileAmount;
    }

    #endregion

    #region Visual

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

    private Sequence SetVisibilityBoardElements(bool visibility)
    {
        return Sequence.Create().Group(SetVisibilityShuffleButton(visibility)).Group(SetVisibilityBoard(visibility));
    }
    
    private async UniTask SetScaleAsync(IEnumerable<Transform> objects, Vector3 cellSize)
    {
        foreach (var obj in objects)
        {
            obj.localScale = cellSize;
            await UniTask.Yield();
        }
    }

    #endregion
}