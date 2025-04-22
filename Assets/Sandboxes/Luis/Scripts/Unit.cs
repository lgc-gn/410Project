using UnityEngine;

/*

UNIT Method Script

Pulls data from inputted UnitData scriptable object



*/

public class Unit : MonoBehaviour
{

    public UnitData data; // Pass in the UnitData scriptableobject

    private string characterName, characterClass;

    private int currentHealth;

    void Start()
    {
        // Initalize all stats
        currentHealth = data.maxHealth;
        characterName = data.characterName;
        characterClass = data.characterClass;

        print($"Created character: {characterName}, {characterClass}");
    }



}
