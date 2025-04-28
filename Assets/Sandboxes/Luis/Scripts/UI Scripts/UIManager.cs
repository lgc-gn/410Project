using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Canvas playerHUD;
    public RectTransform unitInfoTransformDefault; 
    public TMP_Text unitNameText, classText, classActionButtonText, resourceText, hpText, affiliationText;
    public Image classIcon, resourceBar, hpBar;
    public GameObject actionMenu;

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

        unitNameText.SetText(unit.unitData.characterName);

        classText.SetText(unit.classData.className);
        classIcon.sprite = unit.classData.classIcon;
        classActionButtonText.SetText(unit.classData.classAction);

        resourceText.SetText($"{unit.classData.resourceType}: {unit.unitData.currentResource}/{unit.unitData.maxResource}");
        hpText.SetText($"Health: {unit.unitData.currentHealth}/{unit.unitData.maxHealth}");
        resourceBar.color = unit.classData.resourceColor;

        if (unit.unitData.Allied == true)
        {
            affiliationText.SetText("Ally");
        }
        else
        {
            affiliationText.SetText("Enemy");
        }



            StartCoroutine(SmoothMoveInfoUI(unitInfoTransformDefault, 200f, .2f, "up"));


    }

    public void HideUnitInfo()
    {
        //infoPanel.SetActive(false);
        //StartCoroutine(SmoothMoveInfoUI(unitInfoTransformDefault, -200f, .5f, "down"));

    }

    public IEnumerator SmoothMoveInfoUI(RectTransform panel, float distance, float duration, string direction)
    {
        Vector2 startPos = panel.anchoredPosition;
        Vector2 targetPos = startPos;

        if (direction == "up"){
            targetPos = new Vector2(startPos.x, 100f);
        }
        else if (direction == "down")
        {
            targetPos = new Vector2(startPos.x, -100f);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            panel.anchoredPosition = Vector2.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the panel reaches the target position after the loop
        panel.anchoredPosition = targetPos;

    }
}
