using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Tooltip("Rows amount of the table")]
    [SerializeField] int rowsLength = 5;
    [Tooltip("Column amount of the table")]
    [SerializeField] int columnsLength = 5;
    [Tooltip("Margin between cells")]
    [SerializeField] float spacing = 0.1f;
    [Tooltip("The margin of the table to be formed from the right and left axis")]
    [SerializeField] float margin = 0.1f;

    public Tile[,] BoardData { get; private set; }
    private Camera _camera;
    public ObjectPool<BlockData> BlockPool { get; private set; }
    
    private readonly Stack<int> _tempStack = new();
    private readonly List<Tile> _matchedTiles = new();
    private bool[] _visitedCells;

    void Start()
    {
        _camera = Camera.main;
        CreateBoard();
    }
    
    private void OnEnable()
    {
        InputHandler.OnTileClicked += CheckTile;
    }
    private void OnDisable()
    {
        InputHandler.OnTileClicked -= CheckTile;
    }
    
    private void CheckTile(Tile tile)
    {
        if (tile.Data != null)
        {
            var matchedTiles =FloodFill(tile);
            if(matchedTiles.Count < 2) return;

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
            if(fillingColumns.Contains(tile.Column)) continue;
            fillingColumns.Add(tile.Column);
        }

        FillColumns(fillingColumns);
    }

    private void OrderColumn(int columnNum)
    {
        for ( int row = rowsLength - 1; row >= 0; row--) // Sütunun en altından başlayarak yukarı çık
        {
            Tile currentTile = BoardData[row, columnNum];
            if (currentTile.IsTileFilled) continue; // Eğer boş bir kutu bulduysak
            
            for (int upperRow = row - 1; upperRow >= 0; upperRow--) // Daha üst sıralardan dolu bir kutu ara
            {
                Tile upperTile = BoardData[upperRow, columnNum];
                if (!upperTile.IsTileFilled) continue; // Eğer dolu bir kutu bulursak
                BlockManager.Instance.MoveBlock(upperTile, currentTile);
                break;
            }
        }
    }

    public int GetEmptyTileCountOnColumn(int columnNum)
    {
        int emptyTileAmount = 0;
        for ( int row = rowsLength - 1; row >= 0; row--)
        {
            Tile currentTile = BoardData[row, columnNum];
            if(currentTile.IsTileFilled) continue;
            emptyTileAmount++;
        }

        return emptyTileAmount;
    }

    private void FillColumns(List<int> columns)
    {
        foreach (var column in columns)
        {
            OrderColumn(column);
        }

        foreach (var column in columns)
        {
            BlockManager.Instance.SpawnBlock(column);
        }
        
        
    }

    void CreateBoard()
    {
        var tempTileObject = new GameObject().AddComponent<Tile>();
        tempTileObject.gameObject.AddComponent<BoxCollider2D>();
        BoardData = new Tile[rowsLength,columnsLength];
        
        // Get the camera bounds
        float height = 2f * _camera.orthographicSize;
        float width = height * _camera.aspect;

        // Calculate the maximum size for square cells
        float maxCellWidth = (width - 2 * margin - (columnsLength - 1) * spacing) / columnsLength;
        float maxCellHeight = (height - 2 * margin - (rowsLength - 1) * spacing) / rowsLength;

        // Use the smaller dimension for square cells
        float cellSize = Mathf.Min(maxCellWidth, maxCellHeight);

        // Adjust the scale of the cells
        Vector3 cellScale = new Vector3(cellSize, cellSize, 1);
        tempTileObject.transform.localScale = cellScale;
        
        var tempBlockSpriteObject = new GameObject().AddComponent<BlockData>();
        var spriteRenderer = tempBlockSpriteObject.gameObject.AddComponent<SpriteRenderer>();
        tempBlockSpriteObject.transform.localScale = cellScale;
        BlockManager.Instance.SetBlockSize(0.5f);
        BlockPool = new ObjectPool<BlockData>(tempBlockSpriteObject, rowsLength * columnsLength, transform);

        // Calculate starting position to center the grid
        Vector3 startPosition = new Vector3(-((columnsLength - 1) * (cellSize + spacing)) / 2, 
            ((rowsLength - 1) * (cellSize + spacing)) / 2, 0);


        int index = 0;
        // Instantiate grid cells with spacing
        for (int i = 0; i < rowsLength; i++)
        {
            for (int j = 0; j < columnsLength; j++)
            {
                Vector3 position = startPosition + new Vector3(j * (cellSize + spacing), 
                    -i * (cellSize + spacing), 0);
                BoardData[i,j] = Instantiate(tempTileObject, position, Quaternion.identity, transform);
                BoardData[i,j].Init(i,j, BlockManager.Instance.GetRandomBlock());
                index++;
            }
        }
    }

    public List<Tile> FloodFill(Tile startTile)
    {
        InitializeFloodFill();

        int startRow = startTile.Row;
        int startColumn = startTile.Column;
        BlockColor targetColor = startTile.Data.BlockColor;

        _tempStack.Push(ComputeIndex(startRow, startColumn, columnsLength));

        _matchedTiles.Clear();

        while (_tempStack.Count > 0)
        {
            int index = _tempStack.Pop();
            int row = index / columnsLength;
            int column = index % columnsLength;

            if (IsOutOfBounds(row, column) || _visitedCells[index])
                continue;

            Tile currentTile = BoardData[row, column];
            if (!currentTile.IsTileFilled || currentTile.Data.BlockColor != targetColor) continue;
            
            _visitedCells[index] = true;
            _matchedTiles.Add(currentTile);

            AddTilesToStack(row, column);


        }

        return _matchedTiles;
    }

    private void InitializeFloodFill()
    {
        int totalCells = rowsLength * columnsLength;

        if (_visitedCells == null || _visitedCells.Length < totalCells)
            _visitedCells = new bool[totalCells];
        else
            Array.Clear(_visitedCells, 0, totalCells);
    }

    private bool IsOutOfBounds(int row, int column)
    {
        return row < 0 || row >= rowsLength || column < 0 || column >= columnsLength;
    }

    private void AddTilesToStack(int row, int column)
    {
        var directions = new (int rowOffset, int colOffset)[] {
            (-1, 0), // Left
            (1, 0),  // Right
            (0, -1), // Up
            (0, 1)   // Down
        };

        foreach (var (rowOffset, colOffset) in directions)
        {
            int newRow = row + rowOffset;
            int newColumn = column + colOffset;

            if (!IsOutOfBounds(newRow, newColumn))
            {
                _tempStack.Push(ComputeIndex(newRow, newColumn, columnsLength));
            }
        }
    }

    private int ComputeIndex(int row, int column, int columnCount)
    {
        return row * columnCount + column;
    }

    public Tile GetTile(int row, int column)
    {
        if (IsOutOfBounds(row, column))
            return null;

        return BoardData[row, column];
    }
}
