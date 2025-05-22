using UnityEngine;
using UnityEditor;

//use Tools->Assign TileData to Tile-> select scriptable

public class AssignTileData : EditorWindow
{
    TileData tileDataToAssign;

    [MenuItem("Tools/Assign TileData to Tiles")]
    public static void ShowWindow()
    {
        GetWindow<AssignTileData>("Assign TileData");
    }

    void OnGUI()
    {
        GUILayout.Label("Assign TileData to all Tiles without one", EditorStyles.boldLabel);
        
        tileDataToAssign = (TileData)EditorGUILayout.ObjectField("TileData to assign", tileDataToAssign, typeof(TileData), false);

        if (GUILayout.Button("Assign TileData"))
        {
            if (tileDataToAssign == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a TileData asset to assign.", "OK");
                return;
            }

            AssignTileDataToTiles(tileDataToAssign);
        }
    }

    static void AssignTileDataToTiles(TileData data)
    {
        Tile[] allTiles = GameObject.FindObjectsOfType<Tile>();
        int assignedCount = 0;

        foreach (Tile tile in allTiles)
        {
            if (tile.tileData == null)
            {
                Undo.RecordObject(tile, "Assign TileData");
                tile.tileData = data;
                EditorUtility.SetDirty(tile);
                assignedCount++;
            }
        }

        EditorUtility.DisplayDialog("Done", $"Assigned TileData to {assignedCount} tiles.", "OK");
    }
}
