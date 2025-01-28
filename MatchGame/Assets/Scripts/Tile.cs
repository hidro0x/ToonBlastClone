using PrimeTween;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    private Vector2Int _coordinate;
    public BlockData Data { get; private set; }
    public int Row => _coordinate.x;
    public int Column => _coordinate.y;

    public bool IsTileFilled => Data != null;
   
    public void Init(int rowNum, int columnNum, BlockData blockData = null)
    {
        _coordinate = new Vector2Int(rowNum, columnNum);
        AssignBlock(blockData, true);
    }

    public void AssignBlock(BlockData data, bool setPosition)
    {
        if (data != null)
        {
            Data = data;
            if(setPosition)data.gameObject.transform.position = transform.position;
            data.SetSpriteOrder(Row);
        }
    }
    
    public void RemoveBlock()
    {
        BlockManager.Instance.RemoveBlock(Data);
        Data = null;
    }

    public void MarkAsEmpty() => Data = null;
}
