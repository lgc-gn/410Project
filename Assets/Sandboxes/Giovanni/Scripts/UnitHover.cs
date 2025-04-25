using UnityEngine;
using TMPro; // For TextMeshPro

public class UnitHover : MonoBehaviour
{
    private UnitStats stats;
    public TextMeshProUGUI statsText; // Assign StatsText in Inspector

    void Start()
    {
        stats = GetComponent<UnitStats>();
        if (stats == null)
        {
            Debug.LogError("UnitStats component missing on " + gameObject.name);
        }
        if (statsText == null)
        {
            Debug.LogWarning("StatsText not assigned on " + gameObject.name);
        }
        // Clear text initially
        if (statsText != null)
        {
            statsText.text = "";
        }
    }

    void OnMouseEnter()
    {
        if (stats != null && statsText != null)
        {
            statsText.text = stats.GetStats();
        }
    }

    void OnMouseExit()
    {
        if (statsText != null)
        {
            statsText.text = "";
        }
    }
}