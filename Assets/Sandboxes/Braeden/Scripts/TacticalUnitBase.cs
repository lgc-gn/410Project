using UnityEngine;

/*

UNIT Method Script

Holds and initalizes data from passed in UnitData scriptableobject

Does not handle any sort of Unit related actions (move, attacks)

*/

public class TacticalUnitBase : MonoBehaviour
{
    public UnitData unitData; // Pass in the UnitData scriptableobject
    public ClassData classData;
    public Animator animator;
    protected LTacticsMove movementController;

}