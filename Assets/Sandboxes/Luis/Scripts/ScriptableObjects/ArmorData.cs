using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "RPG Objects/Armor Data")]
public class ArmorData : ScriptableObject
{

    public enum ArmorType{
        Helmet,
        Body,
        Accessory
    }


    [Header("Identifiers")]
    public string armorName;
    public string armorDescription;
    public ArmorType armorType;
    public GameObject armorModel;

    [Header("Combat Data")]
    public float baseDefense;

    [Header("Scalings")]
    // Initialize all to 1.0, no scaling provided by default
    // This affects PHYSICAL damage, not spells
    public float physProtection = 1.0f;
    public float magProtection = 1.0f;
}
