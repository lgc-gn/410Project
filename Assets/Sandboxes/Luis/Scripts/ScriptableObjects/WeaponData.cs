using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "RPG Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName, weaponDescription;
    public int baseDamage;
}
