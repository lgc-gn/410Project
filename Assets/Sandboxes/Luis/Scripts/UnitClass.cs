using System;
using System.Collections.Generic;
using UnityEngine;

/*

UNIT CLASS DEFINITION SCRIPT

Defines the variables of the Unit class

*/

public class UnitClass
{
    public string characterName; // name of the character
    public string characterClass; // class of the character
    public float maxMoveDistance = 9f; // How far the unit can move in a turn. 1 Unity unit = 1 meter
    public float currentMoveDistance = 0f; // How much distance the character as moved in a turn. If this >= maxMoveDistance the unit can no longer move in the turn.

    public int strengthStat, dexterityStat, constitutionStat, intelligenceStat, wisdomStat, charismaStat;   // Ability scores


    

}
