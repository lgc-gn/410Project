using UnityEngine;
using System.Collections.Generic; // For List

public class UnitStats : MonoBehaviour
{
    public string unitName = "Rogue";
    public int health = 100;
    public int attack = 20;
    public float speed = 10f;
    [SerializeField] private List<string> skills = new List<string>(); // Editable in Inspector

    void Awake()
    {
        // Default skills if none set in Inspector
        if (skills.Count == 0)
        {
            if (unitName == "Mage")
            {
                skills.Add("Fireball: 30 damage, single target");
                skills.Add("Heal: Restore 20 health");
            }
            else if (unitName == "Fighter")
            {
                skills.Add("Slash: 25 damage, single target");
                skills.Add("Block: Reduce next damage by 50%");
            }
            else if (unitName == "Rogue")
            {
                skills.Add("Backstab: 40 damage, high crit chance");
                skills.Add("Stealth: Avoid next attack");
            }
        }
    }

    public string GetStats()
    {
        return $"Unit: {unitName}\nHealth: {health} Attack: {attack} Speed: {speed}";
    }

    public string GetSkills()
    {
        if (skills.Count == 0)
            return "No skills available";
        return $"Skills:\n- {string.Join("\n- ", skills)}";
    }
}