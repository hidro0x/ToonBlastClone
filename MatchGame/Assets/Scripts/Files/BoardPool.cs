using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardPool 
{
    
    public ObjectPool<Block> BlockPool { get; private set; }
    public ObjectPool<Tile> TilePool { get; private set; }
    
    public async UniTask InitializePoolsAsync(Board board, Vector3 cellSize)
    {
        var amount = board.BoardData.GetLength(0) * board.BoardData.GetLength(1) ;

        if (BlockPool == null)
        {
            var tempBlockSpriteObject = new GameObject().AddComponent<Block>();
            tempBlockSpriteObject.gameObject.AddComponent<SpriteRenderer>();
            tempBlockSpriteObject.transform.localScale = cellSize;
            BlockPool = new ObjectPool<Block>(tempBlockSpriteObject, amount, board.transform);
            tempBlockSpriteObject.gameObject.SetActive(false);
        }

        if (TilePool == null)
        {
            var tempTileObject = new GameObject().AddComponent<Tile>();
            tempTileObject.gameObject.AddComponent<BoxCollider2D>();
            tempTileObject.transform.localScale = cellSize;
            TilePool = new ObjectPool<Tile>(tempTileObject, amount, board.transform);
            tempTileObject.gameObject.SetActive(false);
        }
        
        await BlockPool.EnsurePoolSizeAsync(amount);
        await TilePool.EnsurePoolSizeAsync(amount);
        
        var blockObjects = BlockPool.GetAllObjectsTransforms();
        var tileObjects = TilePool.GetAllObjectsTransforms();
        
        await UniTask.WhenAll(
            SetScaleAsync(blockObjects, cellSize), 
            SetScaleAsync(tileObjects, cellSize) 
        );
    }
    
    private async UniTask SetScaleAsync(IEnumerable<Transform> objects, Vector3 cellSize)
    {
        foreach (var obj in objects)
        {
            obj.localScale = cellSize;
            await UniTask.Yield();
        }
    }
}
