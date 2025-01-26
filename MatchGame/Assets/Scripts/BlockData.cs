using System;
using UnityEngine;

[Serializable]
public class BlockData : MonoBehaviour
{
    public BlockType BlockType { get; private set; }
    public BlockColor BlockColor{ get; private set; }
    
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void ChangeBlock(BlockType type, BlockColor color)
    {
        BlockColor = color;
        BlockType = type;
        _spriteRenderer.sprite = BlockManager.Instance.GetBlockSprite(this);
    }
    

    public void SetSpriteOrder(int row) => _spriteRenderer.sortingOrder = -row;

}

[Serializable]
public struct BlockSprite
{
    [field:SerializeField]public BlockColor Color{ get; private set; }
    [field:SerializeField]public Sprite Sprite { get; private set; }

    public BlockSprite(BlockColor color)
    {
        Color = color;
        Sprite = null;
    }
}


[Serializable]
public enum BlockColor
{
    Red = 0,
    Blue = 1,
    Green = 2,
    Yellow = 3,
    Purple = 4,
    Pink = 5,

}
[Serializable]
public enum BlockType
{
    Default = 0,
    Bomb = 1,
    Rocket = 2,
    Portal = 3,
}
