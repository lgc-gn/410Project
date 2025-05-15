using NUnit.Framework;
using UnityEngine;

/*

Unit Scriptable Object Definition

Creates new "Unit Data" contextual menu item under Create

Stores stats for unit characters.

Referenced in Unit script for hopefully things like movement, combat, levelling etc.

*/


[CreateAssetMenu(fileName = "NewUnitData", menuName = "RPG Objects/Unit Data")]
public class UnitData : ScriptableObject
{

    [Header("Unit Info")]
    public string characterName; // name of the character

    public bool Allied;

    [Header("Turn Info")]
    public bool activeTurn = false;
    public bool isMoving;

    [Header("Resource Info")]
    public int maxHealth;
    public int currentHealth;
    public int maxResource;
    public int currentResource;

    [Header("Level/XP Info")]
    public int currentLevel;
    public float xpNeeded;
    public float xpCurrent;

    public int Initiative; // Testing, determines the unit's position in the turn order

    [Header("Combat Information")]

    public int moveDistance = 5; // How many tiles the unit can move in a turn.
    public int moveSpeed = 5; // How fast the unit moves

    public int attackRange = 1;

    [Header("Ability Scores")]
    public int strengthStat;
    public int dexterityStat, constitutionStat, intelligenceStat, wisdomStat, charismaStat;   // Ability scores, to get it to look pretty in inspector there needs to be a new line idk why

    [Header("Equipment")]
    public WeaponData RightHand;
    public WeaponData LeftHand;

    public ArmorData HeadPiece;
    public ArmorData Armor;

    public ArmorData Accessory1;
    public ArmorData Accessory2;

    


}