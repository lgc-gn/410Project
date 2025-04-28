using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitSelector : MonoBehaviour
{
    private UnitStats stats;
    public TextMeshProUGUI statsText;
    public Image statsBackground;
    public GameObject actionPanel;
    public Button attackButton;
    public Button skill1Button;
    public Button skill2Button;
    public Button moveButton;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI skill1Text;
    public TextMeshProUGUI skill2Text;
    public TextMeshProUGUI moveText;
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
        if (statsBackground == null)
        {
            Debug.LogWarning("StatsBackground not assigned on " + gameObject.name);
        }
        if (actionPanel == null)
        {
            Debug.LogWarning("ActionPanel not assigned on " + gameObject.name);
        }
        // Ensure initial state
        if (statsText != null) statsText.text = "";
        if (statsBackground != null) statsBackground.enabled = false;
        if (actionPanel != null) actionPanel.SetActive(false);
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
        if (selectedUnit != null && selectedUnit != this)
        {
            if (selectedUnit.actionPanel != null)
            {
                selectedUnit.actionPanel.SetActive(false);
            }
        }
        selectedUnit = this;
        if (stats != null && actionPanel != null)
        {
            actionPanel.SetActive(true);
            var actions = stats.GetActions();
            if (attackText != null) attackText.text = actions.Contains("Attack") ? "Attack" : "";
            if (skill1Text != null) skill1Text.text = actions.Count > 1 ? actions[1] : "";
            if (skill2Text != null) skill2Text.text = actions.Count > 2 ? actions[2] : "";
            if (moveText != null) moveText.text = actions.Contains("Move") ? "Move" : "";
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && selectedUnit == this)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != gameObject)
                {
                    if (actionPanel != null)
                    {
                        actionPanel.SetActive(false);
                    }
                    selectedUnit = null;
                }
            }
            else
            {
                if (actionPanel != null)
                {
                    actionPanel.SetActive(false);
                }
                selectedUnit = null;
            }
        }
    }
}