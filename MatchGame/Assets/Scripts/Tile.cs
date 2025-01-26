using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    private Vector2Int _coordinate;
    public BlockData Data { get; private set; }
    public int Row => _coordinate.x;
    public int Column => _coordinate.y;
   
    public void Init(int rowNum, int columnNum, BlockData blockData = null)
    {
        _coordinate = new Vector2Int(rowNum, columnNum);
        AssignBlock(blockData);
    }

    public void AssignBlock(BlockData data)
    {
        if (data != null)
        {
            Data = data;
            data.gameObject.transform.position = transform.position;
            data.SetSpriteOrder(Row);
        }
    }
    
    public void RemoveBlock()
    {
        BlockManager.Instance.RemoveBlock(Data);
        Data = null;
        
    }
}
