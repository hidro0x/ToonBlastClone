using UnityEngine;


public class Block : MonoBehaviour
{
    public BlockType BlockType { get; private set; }
    public BlockData Data { get; private set; }
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Refresh()
    {
        _spriteRenderer.sprite = BlockManager.Instance.GetBlockSprite(this);
    }

    public void ChangeBlock(BlockData data, BlockType type = BlockType.Default)
    {
        Data = data;
        BlockType = type;
        Refresh();
    }

    public void SetSpriteOrder(int row) => _spriteRenderer.sortingOrder = -row;
}
