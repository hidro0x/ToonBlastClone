
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] GameObject cellPrefab;
    [Tooltip("Rows amount of the table")]
    [SerializeField] int rows = 5;
    [Tooltip("Column amount of the table")]
    [SerializeField] int columns = 5;
    [Tooltip("Margin between cells")]
    [SerializeField] float spacing = 0.1f;
    [Tooltip("The margin of the table to be formed from the right and left axis")]
    [SerializeField] float margin = 0.1f;

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        CreateGrid();
    }

    void CreateGrid()
    {
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
        cellPrefab.transform.localScale = cellScale;

        // Calculate starting position to center the grid
        Vector3 startPosition = new Vector3(-((columns - 1) * (cellSize + spacing)) / 2, 
            ((rows - 1) * (cellSize + spacing)) / 2, 0);

        // Instantiate grid cells with spacing
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 position = startPosition + new Vector3(j * (cellSize + spacing), 
                    -i * (cellSize + spacing), 0);
                Instantiate(cellPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}
