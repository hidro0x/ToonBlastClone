using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "Game Settings", menuName = "Settings/New Game Settings", order = 1)]
public class GameSettings : SerializedScriptableObject
{
    [Tooltip("Required numbers for default block.")]
    [field: SerializeField]
    public Vector2Int DefaultIconBlockRange { get; private set; }
    [Tooltip("Required numbers for rocket block.")]
    [field: SerializeField] public Vector2Int FirstIconBlockRange { get; private set; }
    [Tooltip("Required numbers for bomb block.")]
    [field: SerializeField] public Vector2Int SecondIconBlockRange { get; private set; }
    [Tooltip("Required numbers for portal block.")]
    [field: SerializeField] public Vector2Int ThirdIconBlockRange { get; private set; }

    [field: Header("Effects")]
    [field: Space]
    [field: SerializeField]
    [Tooltip("Offset for spawning block from top.")]
    public float BlockSpawnOffset { get; private set; }

    [Tooltip("Speed value for spawning blocks.")]
    [field: SerializeField] public float BlockSpawnFallSpeed { get; private set; }
    [Tooltip("Speed value for ordering blocks.")]
    [field: SerializeField] public float BlockFallSpeed { get; private set; }
    [Tooltip("Curve value for spawning blocks.")]
    [field: SerializeField] public AnimationCurve BlockBounceStrengthCurve { get; private set; }

    [Space] 
    [Tooltip("Block sprites for each color and type.")][Header("Sprites")] [SerializeField]
    private Dictionary<BlockType, BlockSprite[]> _blockSprites =
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

    [Tooltip("All blocks scriptable objects.")]
    [field: SerializeField] public List<BlockData> BlockSO { get; private set; }

    public Dictionary<BlockType, BlockSprite[]> BlockSprites => _blockSprites;
}