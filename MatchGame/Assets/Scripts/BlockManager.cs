using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager : SerializedMonoBehaviour
{
    public static BlockManager Instance { get; private set; }
    private Board _board;


    [Header("Effects")] private float _blockSize;
    [field: SerializeField] public float BlockSpawnOffset { get; private set; }
    [field: SerializeField] public float BlockSpawnFallSpeed { get; private set; }
    [field: SerializeField] public float BlockFallSpeed { get; private set; }
    [field: SerializeField] public float BlockBounceStrength { get; private set; }


    private void Awake()
    {
        _board = GetComponent<Board>();
        Instance = this;
    }
    
    

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

    public BlockData GetRandomBlock()
    {
        var block = _board.BlockPool.Get();
        block.ChangeBlock(BlockType.Default, (BlockColor)Random.Range(0, 6));
        return block;
    }

    public void SpawnBlock(int columnNum)
    {
        List<BlockData> spawnedBlocks = new List<BlockData>();
        var spawnPos = _board.BoardData[0, columnNum].transform.localPosition;
        var spawnAmount = _board.GetEmptyTileCountOnColumn(columnNum);
        
        for (int i = 0; i < spawnAmount; i++)
        {
            var spawnedBlock = GetRandomBlock();
            spawnedBlocks.Add(spawnedBlock);
            spawnedBlock.transform.localPosition = new Vector2(spawnPos.x, spawnPos.y + BlockSpawnOffset + (_blockSize * i) + 0.3f);
        }

        for (int i = spawnAmount - 1; i >= 0; i--)
        {
            MoveBlock(spawnedBlocks[0], _board.BoardData[i, columnNum]);
            spawnedBlocks.RemoveAt(0);
        }
        
    }

    public void RemoveBlock(BlockData data)
    {
        if (data != null)
        {
            _board.BlockPool.Return(data);
        }
    }

    public void MoveBlock(Tile currentTile,Tile targetTile)
    {
        if(targetTile.IsTileFilled) return;
        var movingBlock = currentTile.Data;
        
        currentTile.MarkAsEmpty();
        targetTile.AssignBlock(movingBlock, false);
        
        Tween.LocalPositionY(movingBlock.transform, targetTile.transform.localPosition.y, BlockFallSpeed, Easing.Bounce(BlockBounceStrength));
    }
    
    public void MoveBlock(BlockData movingBlock, Tile targetTile)
    {
        if(targetTile.IsTileFilled) return;
        targetTile.AssignBlock(movingBlock, false);
        Tween.LocalPositionY(movingBlock.transform, targetTile.transform.localPosition.y, BlockSpawnFallSpeed,Easing.Bounce(BlockBounceStrength));
    }
    

    public void SetBlockSize(float f) => _blockSize = f;


}
