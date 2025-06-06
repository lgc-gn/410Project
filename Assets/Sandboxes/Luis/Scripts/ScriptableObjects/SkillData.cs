using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "RPG Objects/Skill Data")]
public class SkillData : ScriptableObject
{

<<<<<<< Updated upstream
    public string skillName, skillDescription;
=======
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
>>>>>>> Stashed changes
    public int skillRange, resourceChange;

    public float skillDamage;
    public SkillType skillType;

<<<<<<< Updated upstream
    public enum SkillType
    {
        Single,
        AoE,
        Self
    }

=======
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


    public bool freeAction;

    [Header("Animation Clips")]
>>>>>>> Stashed changes
    public AnimationClip startupAnim;
    public AnimationClip channelAnim;
    public AnimationClip castAnim;

    public StatusEffectData statusEffect;

    public Sprite skillIcon;
}
