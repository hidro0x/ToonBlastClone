using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Settings/New Block Data", order = 1)]
public class BlockData : SerializedScriptableObject
{
    [field:SerializeField]public BlockColor BlockColor { get; private set; }
}

[Serializable]
public struct BlockSprite
{
    [field: SerializeField] public BlockColor Color { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }

    public BlockSprite( BlockColor color)
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