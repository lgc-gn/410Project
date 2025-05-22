using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(UnitData))]
public class UnitDataRandomizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnitData unitData = (UnitData)target;

        if (GUILayout.Button("Randomize Unit Stats"))
        {
            RandomizeUnitData(unitData);
        }
    }

    private void RandomizeUnitData(UnitData unitData)
    {
        unitData.characterName = "Unit_" + UnityEngine.Random.Range(1, 99);
        //unitData.Allied = Random.value > 0.5f;

        //unitData.Lord = Random.value > 0.9f; // 10% chance to be Lord

        unitData.maxHealth = (int)Math.Round((float)UnityEngine.Random.Range(50, 151),0);
        unitData.currentHealth = unitData.maxHealth;

        unitData.maxResource = (int)Math.Round((float)UnityEngine.Random.Range(10, 101),0);
        unitData.currentResource = unitData.maxResource;

        unitData.currentLevel = (int)Math.Round((float)UnityEngine.Random.Range(1, 11),0);
        unitData.xpNeeded = 100f * unitData.currentLevel;
        unitData.xpCurrent = 0;

        unitData.Initiative = (int)Math.Round((float)UnityEngine.Random.Range(1, 21),0);

        unitData.moveDistance = (int)Math.Round((float)UnityEngine.Random.Range(1, 8),0);
        unitData.moveSpeed = (int)Math.Round((float)UnityEngine.Random.Range(1, 8),0);
        unitData.attackRange = (int)Math.Round((float)UnityEngine.Random.Range(1, 4),0);

        unitData.strengthStat = (int)Math.Round((float)UnityEngine.Random.Range(1, 20),0);
        unitData.dexterityStat = (int)Math.Round((float)UnityEngine.Random.Range(1, 20),0);
        unitData.constitutionStat = (int)Math.Round((float)UnityEngine.Random.Range(1, 20),0);
        unitData.intelligenceStat = (int)Math.Round((float)UnityEngine.Random.Range(1, 20),0);
        unitData.wisdomStat = (int)Math.Round((float)UnityEngine.Random.Range(1, 20),0);
        unitData.charismaStat = (int)Math.Round((float)UnityEngine.Random.Range(1, 20),0);

        // Optional: Assign dummy or null equipment here if needed
        unitData.RightHand = null;
        unitData.LeftHand = null;
        unitData.HeadPiece = null;
        unitData.Armor = null;
        unitData.Accessory1 = null;
        unitData.Accessory2 = null;

        EditorUtility.SetDirty(unitData);
        //Debug.Log("UnitData randomized.");
    }
}
