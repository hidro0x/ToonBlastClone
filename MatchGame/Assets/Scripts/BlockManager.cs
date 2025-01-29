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

    [Header("Game Rules")] [SerializeField]
    private Vector2Int defaultIconBlockRange;

    [SerializeField] private Vector2Int firstIconBlockRange;
    [SerializeField] private Vector2Int secondIconBlockRange;
    [SerializeField] private Vector2Int thirdIconBlockRange;

    [field: Header("Effects")] [field: Space] [SerializeField]
    private float blockSpawnOffset;

    [field: SerializeField] public float BlockSpawnFallTime { get; private set; }
    [field: SerializeField] public float BlockFallTime { get; private set; }
    [SerializeField] private float blockBounceStrength;

    [Space] [SerializeField] private Dictionary<BlockType, BlockSprite[]> blockSprites =
        new Dictionary<BlockType, BlockSprite[]>()
        {
            {
                BlockType.Default,
                new[]
                {
                    new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue),
                    new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink),
                    new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)
                }
            },
            {
                BlockType.Bomb,
                new[]
                {
                    new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue),
                    new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink),
                    new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)
                }
            },
            {
                BlockType.Portal,
                new[]
                {
                    new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue),
                    new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink),
                    new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)
                }
            },
            {
                BlockType.Rocket,
                new[]
                {
                    new BlockSprite(BlockColor.Red), new BlockSprite(BlockColor.Blue),
                    new BlockSprite(BlockColor.Green), new BlockSprite(BlockColor.Pink),
                    new BlockSprite(BlockColor.Purple), new BlockSprite(BlockColor.Yellow)
                }
            },
        };

    public float BlockSize { get; private set; }
    private void Awake()
    {
        _board = GetComponent<Board>();
        Instance = this;
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
            spawnedBlock.transform.localPosition =
                new Vector2(spawnPos.x, spawnPos.y + blockSpawnOffset + (0.2f + (BlockSize * i)));
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
    
    public void MoveBlock(Tile currentTile, Tile targetTile)
    {
        if (targetTile.IsTileFilled) return;
        var movingBlock = currentTile.Data;

        currentTile.MarkAsEmpty();
        targetTile.AssignBlock(movingBlock, false);

        Tween.LocalPositionY(movingBlock.transform, targetTile.transform.localPosition.y, BlockFallTime, Ease.OutQuint);
    }

    public void MoveBlock(BlockData movingBlock, Tile targetTile)
    {
        if (targetTile.IsTileFilled) return;
        targetTile.AssignBlock(movingBlock, false);
        Tween.LocalPositionY(movingBlock.transform, targetTile.transform.localPosition.y, BlockSpawnFallTime,
            Easing.Bounce(blockBounceStrength));
    }


    #region Helpers

    public BlockData GetRandomBlock()
    {
        var block = _board.BlockPool.Get();
        block.ChangeBlock(BlockType.Default, (BlockColor)Random.Range(0, 6));
        return block;
    }
    public Sprite GetBlockSprite(BlockData blockData)
    {
        return blockSprites[blockData.BlockType].FirstOrDefault(s => s.Color == blockData.BlockColor).Sprite;
    }

    public void ShakeBlock(BlockData block)
    {
        Tween.ShakeLocalRotation(block.transform, new Vector3(0, 0, 30), 0.2f);
    }

    public void SetBlockSize(float f) => BlockSize = f;
    public void SetBlockType(BlockData blockData, int amount)
    {
        var blockType = amount switch
        {
            var a when defaultIconBlockRange.x < a && a < defaultIconBlockRange.y => BlockType.Default,
            var a when firstIconBlockRange.x < a && a < firstIconBlockRange.y => BlockType.Rocket,
            var a when secondIconBlockRange.x < a && a < secondIconBlockRange.y => BlockType.Bomb,
            var a when thirdIconBlockRange.x < a && a < thirdIconBlockRange.y => BlockType.Portal,
            _ => throw new ArgumentOutOfRangeException(nameof(amount), amount, null)
        };
        blockData.ChangeBlock(blockType);
    }

    #endregion
    
}