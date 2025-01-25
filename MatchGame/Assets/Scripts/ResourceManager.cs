using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : SerializedMonoBehaviour
{
    [SerializeField] private Dictionary<BlockType, BlockSprite[]> blockSprites = new Dictionary<BlockType, BlockSprite[]>()
    {
        { BlockType.Default ,new[]{new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue), new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink), new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)}},
        { BlockType.Bomb ,new[]{new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue), new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink), new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)}},
        { BlockType.Portal ,new[]{new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue), new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink), new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)}},
        { BlockType.Rocket ,new[]{new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue), new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink), new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)}},
    };

    public Sprite GetBlockSprite(BlockData blockData){
        return blockSprites[blockData.BlockType].FirstOrDefault(s => s.Color == blockData.BlockColor).Sprite;
    }
}
