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

    

    public float maxMoveDistance = 9f; // How far the unit can move in a turn. 1 Unity unit = 1 meter
    public float currentMoveDistance = 0f; // How much distance the character as moved in a turn. If this >= maxMoveDistance the unit can no longer move in the turn.

    public int strengthStat, dexterityStat, constitutionStat, intelligenceStat, wisdomStat, charismaStat;   // Ability scores


    


}