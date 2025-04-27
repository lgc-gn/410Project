using UnityEngine;
using TMPro;
using UnityEngine.UI; // For Image

public class UnitSelector : MonoBehaviour
{
    private UnitStats stats;
    public TextMeshProUGUI statsText; // Bottom screen stats
    public TextMeshProUGUI skillsText; // Right side skills
    public Image statsBackground; // Background for stats
    public Image skillsBackground; // Background for skills
    private static UnitSelector selectedUnit;

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
        if (skillsText == null)
        {
            Debug.LogWarning("SkillsText not assigned on " + gameObject.name);
        }
        if (statsBackground == null)
        {
            Debug.LogWarning("StatsBackground not assigned on " + gameObject.name);
        }
        if (skillsBackground == null)
        {
            Debug.LogWarning("SkillsBackground not assigned on " + gameObject.name);
        }
        // Clear texts and hide backgrounds initially
        if (statsText != null) statsText.text = "";
        if (skillsText != null) skillsText.text = "";
        if (statsBackground != null) statsBackground.enabled = false;
        if (skillsBackground != null) skillsBackground.enabled = false;
    }

    void OnMouseEnter()
    {
        if (stats != null && statsText != null)
        {
            statsText.text = stats.GetStats();
            if (statsBackground != null) statsBackground.enabled = true;
        }
    }

    void OnMouseExit()
    {
        if (statsText != null)
        {
            statsText.text = "";
            if (statsBackground != null) statsBackground.enabled = false;
        }
    }

    void OnMouseDown()
    {
        // Deselect previous unit
        if (selectedUnit != null && selectedUnit != this)
        {
            if (selectedUnit.skillsText != null)
            {
                selectedUnit.skillsText.text = "";
                if (selectedUnit.skillsBackground != null) selectedUnit.skillsBackground.enabled = false;
            }
        }
        // Select this unit
        selectedUnit = this;
        if (stats != null && skillsText != null)
        {
            skillsText.text = stats.GetSkills();
            if (skillsBackground != null) skillsBackground.enabled = true;
        }
    }

    void Update()
    {
        // Deselect if clicking elsewhere
        if (Input.GetMouseButtonDown(0) && selectedUnit == this)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != gameObject)
                {
                    if (skillsText != null)
                    {
                        skillsText.text = "";
                        if (skillsBackground != null) skillsBackground.enabled = false;
                    }
                    selectedUnit = null;
                }
            }
            else
            {
                if (skillsText != null)
                {
                    skillsText.text = "";
                    if (skillsBackground != null) skillsBackground.enabled = false;
                }
                selectedUnit = null;
            }
        }
    }
}