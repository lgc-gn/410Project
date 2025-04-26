using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "RPG Objects/Armor Data")]
public class ArmorData : ScriptableObject
{
    public string armorName;
    public int baseDefense;
}
