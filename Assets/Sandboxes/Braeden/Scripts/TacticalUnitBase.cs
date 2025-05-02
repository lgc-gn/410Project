using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*

UNIT Method Script

Holds and initalizes data from passed in UnitData scriptableobject

Does not handle any sort of Unit related actions (move, attacks)

*/

public class TacticalUnitBase : MonoBehaviour
{
    public List<Action> ActList = new List<Action>();
    public UnitData unitData; // Pass in the UnitData scriptableobject
    public ClassData classData;
    public Animator animator;
    protected LTacticsMove movementController;
    public Unit TargUnit;

}