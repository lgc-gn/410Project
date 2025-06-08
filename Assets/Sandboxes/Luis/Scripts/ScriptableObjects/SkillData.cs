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
    public string skillName;
    [TextArea(5, 20)] public string skillDescription;
    public int skillRange, resourceChange;

    [Header("Damage Stats")]
    public float skillDamage;
    public SkillType skillType;
    public AfflictedResource resourceType = AfflictedResource.Damage;

    public bool freeAction;

    [Header("Animation Clips")]
    public AnimationClip startupAnim;
    public AnimationClip channelAnim;
    public AnimationClip castAnim;
    public float impactMultiplier = .8f;

    [Header("Sound Effects")]
    public AudioClip startupSound;
    public AudioClip channelSound;
    public AudioClip castSound;
    public AudioClip hitSound;


    [Header("Particle Effects")]
    public ParticleSystem startupEffect;
    public ParticleSystem channelEffect;
    public ParticleSystem castEffect;
    public ParticleSystem hitEffect;


    [Header("Status Data")]
    public StatusEffectData statusEffect;

    [Header("Art")]
    public Sprite skillIcon;
}
