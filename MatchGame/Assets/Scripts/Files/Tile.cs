using PrimeTween;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    private Vector2Int _coordinate;
    public Block Block { get; private set; }
    public int Row => _coordinate.x;
    public int Column => _coordinate.y;

    public bool IsTileFilled => Block != null;

    public void Init(int rowNum, int columnNum, Block blockData = null)
    {
        _coordinate = new Vector2Int(rowNum, columnNum);
        AssignBlock(blockData, true);
    }

    public void AssignBlock(Block block, bool setPosition)
    {
        if (block != null)
        {
            Block = block;
            if (setPosition) block.gameObject.transform.position = transform.position;
            block.SetSpriteOrder(Row);
        }
    }

    public void RemoveBlock()
    {
        BlockManager.Instance.RemoveBlock(Block);
        Block = null;
    }

    public void MarkAsEmpty() => Block = null;
}