using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockShuffler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async UniTask ShuffleBoardAsync(Tile[,] boardData)
    {

        int rowLength = boardData.GetLength(0);
        int columnLength = boardData.GetLength(1);
        //Maksimum eşleşen blok sayısı
        var totalMatchBlocks = Random.Range(rowLength, maxExclusive: columnLength);
        //Eşleşen grup büyüklükleri
        var matchGroupSizes = Helpers.GenerateRandomDivisors(totalMatchBlocks);
        


        List<Block> tempBlocksList = new List<Block>();
        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < columnLength; j++)
            {
                tempBlocksList.Add(boardData[i, j].Block);
            }
        }

        for (int i = tempBlocksList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (tempBlocksList[i], tempBlocksList[j]) = (tempBlocksList[j], tempBlocksList[i]);
        }

        int index = 0;
        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < columnLength; j++)
            {
                boardData[i, j].MarkAsEmpty();
                boardData[i, j].AssignBlock(tempBlocksList[index++], true);
                await UniTask.Yield();
            }
        }
    }
    
   
}
