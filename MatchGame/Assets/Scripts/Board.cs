
using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Tooltip("Rows amount of the table")]
    [SerializeField] int rows = 5;
    [Tooltip("Column amount of the table")]
    [SerializeField] int columns = 5;
    [Tooltip("Margin between cells")]
    [SerializeField] float spacing = 0.1f;
    [Tooltip("The margin of the table to be formed from the right and left axis")]
    [SerializeField] float margin = 0.1f;

    private Tile[] _boardData;
    public ObjectPool<BlockData> BlockPool { get; private set; }


    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        CreateBoard();
    }

    void CreateBoard()
    {
        var tempTileObject = new GameObject().AddComponent<Tile>();
        _boardData = new Tile[rows*columns];
        
        // Get the camera bounds
        float height = 2f * _mainCamera.orthographicSize;
        float width = height * _mainCamera.aspect;

        // Calculate the maximum size for square cells
        float maxCellWidth = (width - 2 * margin - (columns - 1) * spacing) / columns;
        float maxCellHeight = (height - 2 * margin - (rows - 1) * spacing) / rows;

        // Use the smaller dimension for square cells
        float cellSize = Mathf.Min(maxCellWidth, maxCellHeight);

        // Adjust the scale of the cells
        Vector3 cellScale = new Vector3(cellSize, cellSize, 1);
        tempTileObject.transform.localScale = cellScale;
        
        var tempBlockSpriteObject = new GameObject().AddComponent<BlockData>();
        tempBlockSpriteObject.gameObject.AddComponent<SpriteRenderer>();
        tempBlockSpriteObject.transform.localScale = cellScale;
        BlockPool = new ObjectPool<BlockData>(tempBlockSpriteObject, rows * columns, transform);

        // Calculate starting position to center the grid
        Vector3 startPosition = new Vector3(-((columns - 1) * (cellSize + spacing)) / 2, 
            ((rows - 1) * (cellSize + spacing)) / 2, 0);


        int index = 0;
        // Instantiate grid cells with spacing
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 position = startPosition + new Vector3(j * (cellSize + spacing), 
                    -i * (cellSize + spacing), 0);
                _boardData[index] = Instantiate(tempTileObject, position, Quaternion.identity, transform);
                _boardData[index].Init(i,j, BlockManager.Instance.GetRandomBlock());
                index++;
            }
        }
    }
}
