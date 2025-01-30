using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Level", menuName = "Settings/New Level", order = 1)]
[Serializable]
public class LevelData : SerializedScriptableObject
{
    [field:HideInInspector][field:SerializeField]public int Row { get; private set; }
    [field:HideInInspector][field:SerializeField]public int Column{ get; private set; }

    [field:HideInInspector][field:SerializeField]public EditorSettings EditorSettings { get; private set; }
    [field:HideInInspector][field:SerializeField]public GameSettings GameSettings { get; private set; }

    [TableMatrix(HorizontalTitle = "Board", DrawElementMethod = "DrawElement", ResizableColumns = false,
        RowHeight = 75)]
    public BlockData[,] Board = new BlockData[0, 0];

    [Button(ButtonSizes.Medium, ButtonStyle.Box, Expanded = true)]
    private void CreateLevel(int row, int column, bool randomness)
    {
        EditorSettings = LoadEditorSettingsFromFolder();
        GameSettings = LoadGameSettingsFromFolder();
        
        Board = new BlockData[row, column];
        Row = row;
        Column = column;
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                if (
                    randomness) 
                {
                    Board[i, j] = GameSettings.BlockSO[Random.Range(0, GameSettings.BlockSO.Count)]; 
                }
                else
                {
                    Board[i, j] = null; 
                }
            }
        }
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }

    public static EditorSettings LoadEditorSettingsFromFolder()
    {
        string folderPath = "Assets/Scripts/Settings"; // Klasör yolu
        string[] guids = AssetDatabase.FindAssets("t:EditorSettings", new[] { folderPath });

        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<EditorSettings>(assetPath);
        }

        Debug.LogWarning("Editor ayarı bulunamadı: " + folderPath);
        return null;
    }
    
    public static GameSettings LoadGameSettingsFromFolder()
    {
        string folderPath = "Assets/Scripts/Settings"; // Klasör yolu
        string[] guids = AssetDatabase.FindAssets("t:GameSettings", new[] { folderPath });

        if (guids.Length > 0)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<GameSettings>(assetPath);
        }

        Debug.LogWarning("Editor ayarı bulunamadı: " + folderPath);
        return null;
    }
    
    


#if UNITY_EDITOR
private static int objectPickerID;
private static Vector2Int selectedCell = new Vector2Int(-1, -1);
private static BlockData DrawElement(Rect rect, BlockData value, BlockData[,] array, int y, int x, LevelData levelData)
    {
        if(objectPickerID == 0) objectPickerID = GUIUtility.GetControlID(FocusType.Passive);
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            selectedCell = new Vector2Int(x, y);
            EditorGUIUtility.ShowObjectPicker<BlockData>(null, false, "", objectPickerID);
            Event.current.Use();
        }

        if (Event.current.commandName == "ObjectSelectorUpdated" &&
            EditorGUIUtility.GetObjectPickerControlID() == objectPickerID)
        {
            BlockData selectedObject = EditorGUIUtility.GetObjectPickerObject() as BlockData;

            if (selectedObject != null && selectedCell == new Vector2Int(x, y))
            {
                array[x, y] = selectedObject;
                EditorUtility.SetDirty(array[x,y]);

                Debug.Log($"Updated cell [{x}, {y}] with {selectedObject.name}");
            }
;
        }

        if (value != null)
        {
            EditorGUI.DrawPreviewTexture(rect.Padding(1), levelData.EditorSettings.EditorBlockSprites[value.BlockColor].texture);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(rect.Padding(1), Texture2D.normalTexture);
        }

        return array[x, y];
    }
#endif
}