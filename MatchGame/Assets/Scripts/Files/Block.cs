using PrimeTween;
using UnityEngine;


public class Block : MonoBehaviour
{
    public BlockType BlockType { get; private set; }
    public BlockData Data { get; private set; }
    private SpriteRenderer _spriteRenderer;

    private Tween _playingAnimation;

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

    #region visual

    public void PlayAnimation(Tween tween)
    {
        if (_playingAnimation.isAlive) _playingAnimation.Stop();
        _playingAnimation = tween;
    }
    
    public void CompleteAnimation()
    {
        if (_playingAnimation.isAlive) _playingAnimation.Complete();
    }

    public void SetSpriteOrder(int row) => _spriteRenderer.sortingOrder = -row;

    #endregion
}
