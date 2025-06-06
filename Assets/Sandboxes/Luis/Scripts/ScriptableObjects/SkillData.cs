using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "RPG Objects/Skill Data")]
public class SkillData : ScriptableObject
{

    public enum SkillType
    {
        Single,
        Area_of_Effect,
        Self
    }

    public enum AfflictedResource
    {
        Damage,
        Heal,
        Mana
    }

    [Header("Skill Information")]
    public string skillName, skillDescription;
    public int skillRange, resourceChange;

    [Header("Damage Stats")]
    public float skillDamage;
    public SkillType skillType;
    public AfflictedResource resourceType = AfflictedResource.Damage;

    public AudioClip hitSound;
    public ParticleSystem hitEffect;

    public bool freeAction;

    [Header("Animation Clips")]
    public AnimationClip startupAnim;
    public AnimationClip channelAnim;
    public AnimationClip castAnim;
    public float impactMultiplier = .8f;

    [Header("Status Data")]
    public StatusEffectData statusEffect;

    [Header("Art")]
    public Sprite skillIcon;
}
