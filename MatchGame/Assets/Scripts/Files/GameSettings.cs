using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "Game Settings", menuName = "Settings/New Game Settings", order = 1)]
public class GameSettings : SerializedScriptableObject
{
    [Header("Game Rules")]
    [field: SerializeField]
    public Vector2Int DefaultIconBlockRange { get; private set; }

    [field: SerializeField] public Vector2Int FirstIconBlockRange { get; private set; }
    [field: SerializeField] public Vector2Int SecondIconBlockRange { get; private set; }
    [field: SerializeField] public Vector2Int ThirdIconBlockRange { get; private set; }

    [field: Header("Effects")]
    [field: Space]
    [field: SerializeField]
    public float BlockSpawnOffset { get; private set; }

    [field: SerializeField] public float BlockSpawnFallSpeed { get; private set; }
    [field: SerializeField] public float BlockFallSpeed { get; private set; }
    [field: SerializeField] public AnimationCurve BlockBounceStrengthCurve { get; private set; }

    [Space] [Header("Sprites")] [SerializeField]
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

    [SerializeField] private Dictionary<BlockColor, Sprite> _editorBlockSprites = new Dictionary<BlockColor, Sprite>();

    [field: SerializeField] public List<BlockData> BlockSO { get; private set; }

    public Dictionary<BlockType, BlockSprite[]> BlockSprites => _blockSprites;
}