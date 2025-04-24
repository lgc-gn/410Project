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

    public string characterName; // name of the character
    public string characterClass; // class of the character

    public int maxHealth;
    public int currentHealth;

    public float xpNeeded;
    public float xpCurrent;

    public int currentLevel;

    

    public int moveDistance = 5; // How many tiles the unit can move in a turn.
    public int moveSpeed = 0; // How fast the unit moves

    public int strengthStat, dexterityStat, constitutionStat, intelligenceStat, wisdomStat, charismaStat;   // Ability scores


    


}