using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClassData", menuName = "RPG Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{

    public enum WeaponType{
        Dagger,
        Spear,
        Sword,
        Axe,
        Greatsword,
        Staff,
        Mace
    }

    [Header("Identifiers")]
    public string weaponName;
    public string weaponDescription;
    public WeaponType weaponType;
    public GameObject weaponModel;

    [Header("Animation Data")]

    public List<AnimationClip> animations;

    [Header("Combat Data")]
    public float baseDamage;
    public float critChance = 5f;
    public float critMultiplier = 1.2f;
    public float flatArmorPen = 0f;
    public float percentArmorPen = 0f;

    [Header("Scalings")]
    // Initialize all to 1.0, no scaling provided by default
    // This affects WEAPON damage, not spells
    public float strScaling = 1.0f;
    public float dexScaling = 1.0f;
    public float intScaling = 1.0f;
    public float wisScaling = 1.0f;


}
