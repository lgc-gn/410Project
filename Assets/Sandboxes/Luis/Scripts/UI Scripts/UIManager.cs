using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Canvas playerHUD;
    public TMP_Text unitNameText;
    public TMP_Text classText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowUnitInfo(Unit unit)
    {
        //print($"Name: {data.characterName}\nClass: {data.characterClass}\nHealth: {data.currentHealth}\nLevel: {data.currentLevel}");
        unitNameText.SetText(unit.data.name);
        classText.SetText(unit.data.characterClass);

    }

    public void HideUnitInfo()
    {
        //infoPanel.SetActive(false);
        print("Unhovered");
    }
}
