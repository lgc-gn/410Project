using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.ProBuilder.MeshOperations;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TurnOrderHandler TurnOrderScript;
    public bool CamState=true;


    public Canvas playerHUD;
    public RectTransform unitInfoTransformDefault, actionMenuTransformDefault, toolTipTransformDefault; 
    public TMP_Text unitNameText, classText, classActionButtonText, resourceText, hpText, xpText, lvlText, affiliationText, toolTipText;
    public Image classIcon, unitPortrait, resourceBar, hpBar, xpBar;
    public GameObject actionMenu, unitEntryPrefab, dmgNumberPrefab;
    public Transform turnOrderPanel, actionMenuPanel;

    public GameObject[] cameras;  // Assign in Inspector
    public GameObject primaryCam;
    private int currentIndex = 0;

    public GameObject ActionBoard;
    public GameObject StatsBoard;
    [Header("Action Menu Button")]
    public Button attackButton;
    public Button moveButton;
    public Button statusButton;
    public Button classActionButton;
    public Button endTurnButton;
    public Button camResetButton;


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
        camResetButton.onClick.AddListener(OnResetClicked);
        endTurnButton.onClick.AddListener(OnEndTurnClicked);

        UpdateActiveCamera();
    }

    #region Player Button Input

    void OnAttackClicked()
    {
        Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();
        if (currentUnit.unitData.Allied == true)
            currentUnit.HandleStateTransition(Unit.UnitState.Attack_Check);

    }

    void OnMoveClicked()
    {
        Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();
        if (currentUnit.unitData.Allied == true)
            currentUnit.HandleStateTransition(Unit.UnitState.Move_Check);
    }

    void OnStatusClicked()
    {
        Debug.Log("Status clicked!");
        // Show status panel
    }

    void OnClassActionClicked()
    {
        Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();
        AttackHoverDetection AOEhover = this.GetComponent<AttackHoverDetection>();
        if (currentUnit.unitData.Allied == true)
        {
            AOEhover.currUnit = currentUnit;
        }
    }

    public void OnResetClicked()
    {
        cameras[currentIndex].SetActive(false);
        CamState = !CamState;
        currentIndex = (currentIndex + 1) % cameras.Length;
        cameras[currentIndex].SetActive(true);
        primaryCam = cameras[currentIndex];
        HoverDetection hover = this.GetComponent<HoverDetection>();
        //hover.enabled = false;

        if (currentIndex % cameras.Length == 0)
        {
            ActionBoard.SetActive(true);
            StatsBoard.SetActive(true);
            //hover.enabled = false;
        }
        else
        {
            ActionBoard.SetActive(false);
            //hover.enabled = true;
        }

        Debug.Log("Switched to camera: " + cameras[currentIndex].name);
        Debug.Log(currentIndex);
    }

    void UpdateActiveCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            bool shouldBeActive = (i == currentIndex);
            cameras[i].SetActive(shouldBeActive);

            // Optional: Also ensure Camera component is enabled
            Camera cam = cameras[i].GetComponent<Camera>();
            if (cam != null)
            {
                cam.enabled = shouldBeActive;
            }
        }
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

    public IEnumerator DisplayToolTip(string direction, float duration, string textToolTip)
    {
        Vector2 startPos = toolTipTransformDefault.anchoredPosition;
        Vector2 targetPos = startPos;
        Vector2 defaultPos = new Vector2(0, 60.45f);

        toolTipText.SetText(textToolTip);

        if (direction == "down")
        {
            targetPos = new Vector2(startPos.x, -100f);
        }
        else if (direction == "up")
        {
            targetPos = new Vector2(startPos.x, 100f);
        }
        else if (direction == "neutral")
        {
            targetPos = defaultPos;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            toolTipTransformDefault.anchoredPosition = Vector2.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the panel reaches the target position after the loop
        toolTipTransformDefault.anchoredPosition = targetPos;

    }

    public void IdleUIState()
    {
        StartCoroutine(DisplayToolTip("up", .1f, "..."));
        StartCoroutine(SmoothMoveActionUI("left", .15f));
    }

    public void AorMUIState()
    {
        StartCoroutine(SmoothMoveActionUI("right", .15f));
    }

    public void NoToolTipState()
    {
        StartCoroutine(DisplayToolTip("up", .1f, "..."));
        StartCoroutine(SmoothMoveActionUI("right", .15f));
    }



    public void ConfirmState()
    {
        
    }

    public void UpdateTurnOrderList(Queue<Unit> currentTurnOrder)
    {
        foreach (Transform child in turnOrderPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Unit unit in currentTurnOrder)
        {
            if (unit.unitData.Dead == false)
            {
                GameObject entry = Instantiate(unitEntryPrefab, turnOrderPanel, false);
                entry.GetComponent<Image>().color = unit.unitData.Allied ? allyColor : enemyColor;
                entry.transform.Find("UnitName").GetComponent<TMP_Text>().SetText(unit.unitData.characterName);
                entry.transform.Find("UnitClass").GetComponent<TMP_Text>().SetText($"Lvl. {unit.unitData.currentLevel} {unit.classData.className}");
                entry.transform.Find("ClassIcon").GetComponent<Image>().sprite = unit.classData.classIcon;
            }

        }
    }

    public void DisplayDamageNumber(float DamageValue, Unit Target)
    {
        GameObject DamageNumber = Instantiate(dmgNumberPrefab, Target.transform.position, Quaternion.identity, Target.transform);
        DamageNumber.GetComponent<TextMeshPro>().SetText($"{DamageValue}");

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

    //private void Update()
    //{
    //    Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();
    //    if (currentUnit.unitData.Allied == false)
    //    {
    //        AorMUIState();
    //    }
    //}

    #endregion
}
