using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TurnOrderHandler TurnOrderScript;

    public Canvas playerHUD;
    public RectTransform unitInfoTransformDefault, actionMenuTransformDefault; 
    public TMP_Text unitNameText, classText, classActionButtonText, resourceText, hpText, xpText, lvlText, affiliationText;
    public Image classIcon, unitPortrait, resourceBar, hpBar, xpBar;
    public GameObject actionMenu, unitEntryPrefab;
    public Transform turnOrderPanel, actionMenuPanel;

    [Header("Action Menu Button")]
    public Button attackButton;
    public Button moveButton;
    public Button statusButton;
    public Button classActionButton;
    public Button endTurnButton;

    [SerializeField] private Color enemyColor, allyColor;
    [SerializeField] private Sprite enemySprite, allySprite;

    private enum WarningType
    {
        EndTurn,
        Move,
        Attack
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        attackButton.onClick.AddListener(OnAttackClicked);
        moveButton.onClick.AddListener(OnMoveClicked);
        statusButton.onClick.AddListener(OnStatusClicked);
        classActionButton.onClick.AddListener(OnClassActionClicked);
        endTurnButton.onClick.AddListener(OnEndTurnClicked);
    }

    #region Player Button Input

    void OnAttackClicked()
    {
        DisplayConfirm(WarningType.Attack);
        //StartCoroutine(SmoothMoveActionHelperUI("down", .1f));


    }

    void OnMoveClicked()
    {
        DisplayConfirm(WarningType.Move);
        //StartCoroutine(TurnOrderScript.ReturnCurrentQueue().Peek().HandleMoveRoutine());
        //StartCoroutine(SmoothMoveActionHelperUI("down", .1f));

    }

    void OnStatusClicked()
    {
        Debug.Log("Status clicked!");
        // Show status panel
    }

    void OnClassActionClicked()
    {

    }

    void OnEndTurnClicked()
    {
        //DisplayConfirm(WarningType.EndTurn);
        TurnOrderScript.ReturnCurrentQueue().Peek().EndTurn();
    }


    #endregion

    #region Utilities

    public IEnumerator SmoothMoveActionUI(string direction, float duration)
    {
        Vector2 startPos = actionMenuTransformDefault.anchoredPosition;
        Vector2 targetPos = startPos;

        if (direction == "left"){
            targetPos = new Vector2(-100f, startPos.y);
        }
        else if (direction == "right")
        {
            targetPos = new Vector2(100f, startPos.y);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            actionMenuTransformDefault.anchoredPosition = Vector2.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the panel reaches the target position after the loop
        actionMenuTransformDefault.anchoredPosition = targetPos;

    }



    private void DisplayConfirm(WarningType warning)
    {
        switch (warning)
        {
            case WarningType.EndTurn:
                print("This will end your turn, confirm?");
                break;
            case WarningType.Move:

                break;
            case WarningType.Attack:

                break;
        }
    }

    public void UpdateTurnOrderList(Queue<Unit> currentTurnOrder)
    {
        foreach (Transform child in turnOrderPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Unit unit in currentTurnOrder)
        {
            GameObject entry = Instantiate(unitEntryPrefab, turnOrderPanel, false);
            entry.GetComponent<Image>().color = unit.unitData.Allied ? allyColor : enemyColor;
            entry.transform.Find("UnitName").GetComponent<TMP_Text>().SetText(unit.unitData.characterName);
            entry.transform.Find("UnitClass").GetComponent<TMP_Text>().SetText($"Lvl. {unit.unitData.currentLevel} {unit.classData.className}");
            entry.transform.Find("ClassIcon").GetComponent<Image>().sprite = unit.classData.classIcon;

        }
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
        //xpText.SetText($"Experience: {unit.unitData.xpCurrent}/{unit.unitData.xpNeeded}");
        //lvlText.SetText($"Level: {unit.unitData.currentLevel}");


        resourceBar.color = unit.classData.resourceColor;

        if (unit.unitData.Allied == true)
        {
            affiliationText.SetText("Ally");
            unitPortrait.sprite = allySprite;

        }
        else
        {
            affiliationText.SetText("Enemy");
            unitPortrait.sprite = enemySprite;
        }

    }

    #endregion
}
