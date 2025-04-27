using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*

UNIT Method Script

Pulls data from inputted UnitData scriptable object



*/

public class TacticalUnitBase : LTacticsMove
{
    public UnitData data; // Pass in the UnitData scriptableobject
    public Animator animator;

    public string characterName, characterClass;


    public int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        animator = GetComponent<Animator>();

        init(data, animator);
        currentHealth = data.maxHealth;
        characterName = data.characterName;
        characterClass = data.characterClass;
        Debug.Log("Unit speed set to: " + moveSpeed);

        print($"Loaded character: {characterName}, {characterClass}");
    }
}