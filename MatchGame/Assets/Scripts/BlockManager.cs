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
    
    [field:SerializeField] public GameSettings Settings { get; private set; }
    

    public float BlockSize { get; private set; }
    private void Awake()
    {
        _board = GetComponent<Board>();
        Instance = this;
    }
    
    public void SpawnBlock(int columnNum)
    {
        List<Block> spawnedBlocks = new List<Block>();
        var spawnPos = _board.BoardData[0, columnNum].transform.localPosition;
        var spawnAmount = _board.GetEmptyTileCountOnColumn(columnNum);

        for (int i = 0; i < spawnAmount; i++)
        {
            var spawnedBlock = GetRandomBlock();
            spawnedBlocks.Add(spawnedBlock);
            spawnedBlock.transform.localPosition =
                new Vector2(spawnPos.x, spawnPos.y + Settings.BlockSpawnOffset + (0.5f*i) + (BlockSize * i));
        }

        for (int i = spawnAmount - 1; i >= 0; i--)
        {
            
            MoveBlock(spawnedBlocks[0], _board.BoardData[i, columnNum]);
            spawnedBlocks.RemoveAt(0);
        }
    }
    
    public void RemoveBlock(Block block)
    {
        if (block != null)
        {
            _board.BlockPool.Return(block);
            // var localScale = block.transform.localScale;
            // block.PlayAnimation(Tween.Scale(block.transform, 0 ,0.1f, Ease.InQuint).OnComplete(delegate
            // {
            //     _board.BlockPool.Return(block);
            //     block.transform.localScale = localScale;
            // }));
        }
    }
    
    public void MoveBlock(Tile currentTile, Tile targetTile)
    {
        if (targetTile.IsTileFilled) return;
        var movingBlock = currentTile.Block;

        currentTile.MarkAsEmpty();
        targetTile.AssignBlock(movingBlock, false);

        movingBlock.PlayAnimation(Tween.LocalPositionAtSpeed(movingBlock.transform, targetTile.transform.localPosition, Settings.BlockFallTime,
            Ease.OutQuint));
    }

    public void MoveBlock(Block movingBlock, Tile targetTile)
    {
        if (targetTile.IsTileFilled) return;
        targetTile.AssignBlock(movingBlock, false);

        //movingBlock.PlayAnimation(Tween.LocalPositionAtSpeed(movingBlock.transform, targetTile.transform.localPosition, Settings.BlockSpawnFallTime, Easing.Bounce(Settings.BlockBounceStrength)));
        movingBlock.PlayAnimation(Tween.LocalPositionAtSpeed(movingBlock.transform, targetTile.transform.localPosition, Settings.BlockSpawnFallTime, Settings.BlockBounceStrengthCurve));
    }


    #region Helpers

    // public Block GetBlock(BlockType type, BlockColor color)
    // {
    //     var block = _blocksSO[type].First(x => x.Data.BlockColor == color);
    //     return block;
    // }
    
    public Block GetRandomBlock()
    {
        var block = _board.BlockPool.Get();
        block.ChangeBlock(Settings.BlockSO[Random.Range(0,6)]);
        return block;
    }
    
    public Block GetBlock(BlockColor color, BlockType type = BlockType.Default)
    {
        var block = _board.BlockPool.Get();
        block.ChangeBlock(Settings.BlockSO.First(x=>x.BlockColor == color), type);
        return block;
    }

    public void ShakeBlock(Block block)
    {
        Tween.ShakeLocalRotation(block.transform, new Vector3(0, 0, 30), 0.2f);
    }

    public void SetBlockSize(float f) => BlockSize = f;
    public void SetBlockType(Block block, int amount)
    {
        var blockType = amount switch
        {
            var a when Settings.DefaultIconBlockRange.x < a && a < Settings.DefaultIconBlockRange.y => BlockType.Default,
            var a when Settings.FirstIconBlockRange.x < a && a < Settings.FirstIconBlockRange.y => BlockType.Rocket,
            var a when Settings.SecondIconBlockRange.x < a && a < Settings.SecondIconBlockRange.y =>BlockType.Bomb,
            var a when Settings.ThirdIconBlockRange.x < a && a < Settings.ThirdIconBlockRange.y => BlockType.Portal,
            _ => throw new ArgumentOutOfRangeException(nameof(amount), amount, null)
        };
        block.ChangeBlock(block.Data, blockType);
    }

    public Sprite GetBlockSprite(Block block)
    {
        foreach (var blockSprite in Settings.BlockSprites[block.BlockType])
        {
            if(blockSprite.Color !=  block.Data.BlockColor) continue;
            return blockSprite.Sprite;
        }

        return null;
    }

    #endregion
    
}