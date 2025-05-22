using NUnit.Framework;
using UnityEngine;

/*

Unit Scriptable Object Definition

Creates new "Unit Data" contextual menu item under Create

Stores stats for unit characters.

Referenced in Unit script for hopefully things like movement, combat, levelling etc.

*/


[CreateAssetMenu(fileName = "NewTileData", menuName = "RPG Objects/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Tile Info")]
    public string tileType; // type of tile
    public bool walk; // sets walkability
    
    // Add this:
    public Color tileColor = Color.white; // default color is white
}
