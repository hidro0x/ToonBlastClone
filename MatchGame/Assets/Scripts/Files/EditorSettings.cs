using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[CreateAssetMenu(fileName = "Editor Settings", menuName = "Settings/New Editor Settings", order = 1)]
public class EditorSettings : SerializedScriptableObject
{
    [Tooltip("Block sprites for level editor.")]
    [SerializeField] private Dictionary<BlockColor, Sprite> editorBlockSprites = new Dictionary<BlockColor, Sprite>();
    public Dictionary<BlockColor, Sprite> EditorBlockSprites => editorBlockSprites;

}
