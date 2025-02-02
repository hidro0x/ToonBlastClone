using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    [Header("Board Settings")] [SerializeField]
    private LevelData level;

    private int _rowsLength;
    private int _columnsLength;

    public Tile[,] BoardData { get; private set; }

    private BoardUI _boardUI;
    public BoardPool BoardPool{ get; private set; }


    private readonly List<Tile> _matchedTiles = new();
    private readonly Stack<int> _tempStack = new();
    private bool[] _visitedCells;
    
    private bool _canShuffle = true;

    #region MonoBehaviour

    async void Start()
    {
        InputHandler.OnTileClicked += CheckTile;

        _boardUI = GetComponent<BoardUI>();
        BoardPool = new BoardPool();
        
        await CreateBoard(level);
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

        await _boardUI.SetVisibilityBoardElements(false);

        ShuffleBoard();

        await _boardUI.SetVisibilityBoardElements(true);

        InputHandler.OnControlInput?.Invoke(true);
        _canShuffle = true;
    }

    
    private void ShuffleBoard()
    {
        System.Random random = new System.Random();

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
        CheckBlockGroups();
    }

    private void HandleDeadlock()
    {
        var floodCounts = Helpers.GenerateRandomDivisors((int)Random.Range(2, maxInclusive: Math.Max(_rowsLength, _columnsLength)));
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
        while (_tempStack.Count > 0)
        {
            if(floodCount == 0) return;
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
            var matchedTiles = GetMatchingTiles(tile);
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
                var matchedTiles = GetMatchingTiles(BoardData[i, j]);
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
    
    private void FillColumns(List<int> columns)
    {
        foreach (var column in columns)
        {
            OrderColumn(column);
            BlockManager.Instance.SpawnBlock(column);
        }

        CheckBlockGroups();

        if (!IsBoardPlayable())
        { 
            StartShuffle();
        }
        
    }

    private async UniTask CreateBoard(LevelData data = null)
    {
        _rowsLength = data == null ? 10 : level.Row;
        _columnsLength = data == null ? 9 : level.Column;
        
        BoardData = new Tile[_rowsLength, _columnsLength];

        await _boardUI.CreateBoard(this);
        
        for (int i = 0; i < _rowsLength; i++)
        {
            for (int j = 0; j < _columnsLength; j++)
            {
                BoardData[i, j].Init(i, j,
                    level == null
                        ? BlockManager.Instance.GetRandomBlock()
                        : BlockManager.Instance.GetBlock(level.Board[i, j].BlockColor));
            }
        }

        if (!IsBoardPlayable())
        {
            HandleDeadlock();
        }
        
        CheckBlockGroups();
        
        await _boardUI.SetVisibilityBoardElements(true);
    }
    

    private List<Tile> GetMatchingTiles(Tile startTile)
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
    
}