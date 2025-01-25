using System;
using UnityEngine;

[Serializable]
public class BlockData
{
    public BlockType BlockType { get; private set; }
    public BlockColor BlockColor{ get; private set; }
    public BlockData(BlockColor blockColor, BlockType blockType)
    {
        BlockColor = blockColor;
        BlockType = blockType;
    }
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
