using UnityEngine;
using UnityEditor;

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
        unitData.characterName = "Unit_" + Random.Range(1000, 9999);
        unitData.Allied = Random.value > 0.5f;

        unitData.Lord = Random.value > 0.9f; // 10% chance to be Lord

        unitData.maxHealth = Random.Range(50, 151);
        unitData.currentHealth = Random.Range(0, unitData.maxHealth + 1);

        unitData.maxResource = Random.Range(10, 101);
        unitData.currentResource = Random.Range(0, unitData.maxResource + 1);

        unitData.currentLevel = Random.Range(1, 11);
        unitData.xpNeeded = 100f * unitData.currentLevel;
        unitData.xpCurrent = Random.Range(0f, unitData.xpNeeded);

        unitData.Initiative = Random.Range(1, 21);

        unitData.moveDistance = Random.Range(3, 10);
        unitData.moveSpeed = Random.Range(3, 10);
        unitData.attackRange = Random.Range(1, 4);

        unitData.strengthStat = Random.Range(1, 20);
        unitData.dexterityStat = Random.Range(1, 20);
        unitData.constitutionStat = Random.Range(1, 20);
        unitData.intelligenceStat = Random.Range(1, 20);
        unitData.wisdomStat = Random.Range(1, 20);
        unitData.charismaStat = Random.Range(1, 20);

        // Optional: Assign dummy or null equipment here if needed
        unitData.RightHand = null;
        unitData.LeftHand = null;
        unitData.HeadPiece = null;
        unitData.Armor = null;
        unitData.Accessory1 = null;
        unitData.Accessory2 = null;

        EditorUtility.SetDirty(unitData);
        Debug.Log("UnitData randomized.");
    }
}
