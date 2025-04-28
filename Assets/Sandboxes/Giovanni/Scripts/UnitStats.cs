using UnityEngine;
using System.Collections.Generic;

public class UnitStats : MonoBehaviour
{
    public string unitName = "Mage";
    public int health = 100;
    public int attack = 20;
    public float speed = 10f;
    [SerializeField] private List<string> actions = new List<string>();

    void Awake()
    {
        // Set default actions
        if (actions.Count == 0)
        {
            if (unitName == "Mage")
            {
                actions.Add("Attack");
                actions.Add("Fireball");
                actions.Add("Heal");
                actions.Add("Move");
            }
            else if (unitName == "Fighter")
            {
                actions.Add("Attack");
                actions.Add("Slash");
                actions.Add("Block");
                actions.Add("Move");
            }
            else if (unitName == "Rogue")
            {
                actions.Add("Attack");
                actions.Add("Backstab");
                actions.Add("Stealth");
                actions.Add("Move");
            }
        }
    }

    public string GetStats()
    {
        return $"Unit: {unitName}\nHealth: {health} Attack: {attack} Speed: {speed}";
    }

    public List<string> GetActions()
    {
        return actions.Count > 0 ? actions : new List<string> { "No actions available" };
    }
}