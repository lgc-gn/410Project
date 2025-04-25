using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public string unitName = "Mage"; // Editable in Inspector
    public int health = 100;
    public int attack = 20;
    public float speed = 10f; // For turn order

    public string GetStats()
    {
        return $"Unit: {unitName}\nHealth: {health} Attack: {attack} Speed: {speed}";
    }
}