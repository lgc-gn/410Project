using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "RPG Objects/Skill Data")]
public class SkillData : ScriptableObject
{

    public string skillName, skillDescription;
    public int skillRange, resourceChange;

    public float skillDamage;
    public SkillType skillType;

    public enum SkillType
    {
        Single,
        AoE,
        Self
    }

    public AnimationClip startupAnim;
    public AnimationClip channelAnim;
    public AnimationClip castAnim;

    public StatusEffectData statusEffect;

    public Sprite skillIcon;
}
