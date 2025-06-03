using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "RPG Objects/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string statusName, statusDescription;
    public int baseDuration, remainingDuration;
    public Sprite statusIcon;
}
