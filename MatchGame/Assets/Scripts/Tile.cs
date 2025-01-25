using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    public Vector2Int Coordinate { get; private set; }
    public BlockData BlockData { get; private set; }

    public void Init(int rowNum, int columnNum, BlockData blockData = null)
    {
        Coordinate = new Vector2Int(rowNum, columnNum);
    }
}
