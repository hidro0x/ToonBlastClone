using UnityEngine;
using UnityEngine.PlayerLoop;

public class Tile : MonoBehaviour
{
    public int RowNum { get; private set; }
    public int ColumnNum{ get; private set; }

    public void Init(int rowNum, int columnNum)
    {
        RowNum = rowNum;
        ColumnNum = columnNum;
    }
}
