using UnityEngine;

public class ActiveStatusEffect
{
    public StatusEffectData data;
    public int remainingDuration;

    public ActiveStatusEffect(StatusEffectData data)
    {
        this.data = data;
        this.remainingDuration = data.baseDuration;
    }

    public void DecayDuration()
    {
        remainingDuration -= 1;
    }

    public bool IsExpired()
    {
        return remainingDuration <= 0;
    }
}
