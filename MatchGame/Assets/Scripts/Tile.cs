using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    public Vector2Int Coordinate { get; private set; }
    public BlockData Data { get; private set; }
   
    public void Init(int rowNum, int columnNum, BlockData blockData = null)
    {
        Coordinate = new Vector2Int(rowNum, columnNum);
        AssignBlock(blockData);
    }

    public void AssignBlock(BlockData data)
    {
        if (data != null)
        {
            Data = data;
            data.gameObject.transform.position = transform.position;
            data.SetSpriteOrder(Coordinate.x);
        }
    }
}
