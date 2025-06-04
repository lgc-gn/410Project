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
    public GameObject actionMenu, unitEntryPrefab, dmgNumberPrefab, targetCombatPanel, unitInfoPanel, confirmButtonPanel;
    public Transform turnOrderPanel, actionMenuPanel;

    public GameObject VictoryOverlay;

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
    public Button confirmButton;

    [Header("Status Menu Objects")]
    public Color statusMenuEnabled;
    public Color statusMenuDisabled;
    public TMP_Text strAttributeNumber;
    public TMP_Text dexAttributeNumber, conAttributeNumber , intAttributeNumber, wisAttributeNumber, chrAttributeNumber;
    private bool statusMenuOpen = false;

    [Header("Skill Menu Objects")]
    private bool skillMenuOpen = false;
    public GameObject skillMenuPanel;
    public GameObject skillListPrefab;
    public Transform skillListPanel;

    [Header("Target Menu Objects")]
    public TMP_Text sourceHealth;
    public TMP_Text sourceResource, targetHealth, targetResource;
    public Image sourceHealthBar, sourceResourceBar, targetHealthBar, targetResourceBar;

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
        StatusMenuState("up");
        statusMenuOpen = true;
        
    }

    void OnClassActionClicked()
    {
        Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();
        //AttackHoverDetection AOEhover = this.GetComponent<AttackHoverDetection>();
        if (currentUnit.unitData.Allied == true)
        {
            SkillMenuState("enabled");
            //AOEhover.currUnit = currentUnit;
        }
    }

    public void OnResetClicked()
    {
        Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();
        cameras[currentIndex].SetActive(false);
        CamState = !CamState;
        currentIndex = (currentIndex + 1) % cameras.Length;
        cameras[currentIndex].SetActive(true);
        primaryCam = cameras[currentIndex];
        HoverDetection hover = GetComponent<HoverDetection>();
        //hover.enabled = false;

        if (currentIndex % cameras.Length == 0)
        {
            ActionBoard.SetActive(true);
            hover.hoverState = false;
            hover.defaultUni = currentUnit;
            //hover.enabled = false;
        }
        else
        {
            ActionBoard.SetActive(false);
            Debug.Log(primaryCam.name);
            hover.hoverState = true;
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
            targetPos = new Vector2(-150f, startPos.y);
        }
        else if (direction == "right")
        {
            targetPos = new Vector2(150f, startPos.y);
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

    public IEnumerator SmoothMoveStatusMenu(string direction, float duration)
    {
        Vector2 startPos = unitInfoTransformDefault.anchoredPosition;
        Vector2 targetPos = startPos;

        if (direction == "up")
        {
            targetPos = new Vector2(startPos.x, 800f);
        }
        else if (direction == "down")
        {
            targetPos = new Vector2(startPos.x, 100f);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            unitInfoTransformDefault.anchoredPosition = Vector2.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        unitInfoTransformDefault.anchoredPosition = targetPos;

    }

    public IEnumerator DisplayToolTip(string direction, float duration, string textToolTip)
    {
        Vector2 startPos = toolTipTransformDefault.anchoredPosition;
        Vector2 targetPos = startPos;
        Vector2 defaultPos = new Vector2(0, 100f);

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

    public void StatusMenuState(string direction)
    {
        if (direction == "up")
        {
            StartCoroutine(SmoothMoveActionUI("right", .1f));
            StartCoroutine(SmoothMoveStatusMenu(direction, .1f));
        }
        else if (direction == "down")
        {
            StartCoroutine(SmoothMoveActionUI("left", .1f));
            StartCoroutine(SmoothMoveStatusMenu(direction, .1f));
        }
    }

    public void SkillMenuState(string enabled)
    {
        if (enabled == "enabled")
        {
            StartCoroutine(SmoothMoveActionUI("right", .1f));
            LoadSkillMenu();
            skillMenuPanel.SetActive(true);
            skillMenuOpen = true;
        }
        else if(enabled == "disabled")
        {
            StartCoroutine(SmoothMoveActionUI("left", .1f));
            skillMenuPanel.SetActive(false);
            skillMenuOpen = false;
        }
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
                entry.transform.Find("HealthFrame").Find("HealthBar").GetComponent<Image>().fillAmount = (float)unit.unitData.currentHealth / unit.unitData.maxHealth;
            }

        }
    }

    public void LoadSkillMenu()
    {
        Unit currentUnit = TurnOrderScript.ReturnCurrentQueue().Peek();

        foreach (Transform item in skillListPanel)
        {
            Destroy(item.gameObject);
        }

        foreach(SkillData skill in currentUnit.unitData.KnownSkills)
        {
            GameObject entry = Instantiate(skillListPrefab, skillListPanel, false);
            entry.transform.Find("Icon").GetComponent<Image>().sprite = skill.skillIcon;
            entry.transform.Find("SkillName").GetComponent<TMP_Text>().SetText(skill.skillName);
            entry.transform.Find("SkillDescription").GetComponent<TMP_Text>().SetText(skill.skillDescription);

            if (skill.resourceType == SkillData.AfflictedResource.Damage) {
                entry.transform.Find("Damage").GetComponent<TMP_Text>().SetText("Damage");
                entry.transform.Find("Damage").Find("dmgText").GetComponent<TMP_Text>().SetText(skill.skillDamage.ToString());
            }
            else if (skill.resourceType == SkillData.AfflictedResource.Heal) {
                entry.transform.Find("Damage").GetComponent<TMP_Text>().SetText("Heal");
                entry.transform.Find("Damage").Find("dmgText").GetComponent<TMP_Text>().SetText("+"+skill.skillDamage.ToString());
            }
            else if (skill.resourceType == SkillData.AfflictedResource.Mana)
            {
                entry.transform.Find("Damage").GetComponent<TMP_Text>().SetText("Mana");
                entry.transform.Find("Damage").Find("dmgText").GetComponent<TMP_Text>().SetText("+" + skill.skillDamage.ToString());
            }

            if (skill.statusEffect != null)
                entry.transform.Find("Effect").Find("effectText").GetComponent<TMP_Text>().SetText(skill.statusEffect.statusName);
            else
                entry.transform.Find("Effect").Find("effectText").GetComponent<TMP_Text>().SetText("N/A");

            entry.transform.Find("Cost").Find("costText").GetComponent<TMP_Text>().SetText(skill.resourceChange.ToString());

            SkillData capturedSkill = skill;
            if (currentUnit.unitData.attackedThisTurn != true)
            {
                Button btn = entry.GetComponent<Button>();
                btn.onClick.AddListener(() => {
                    currentUnit.OnSkillSelected(capturedSkill);
                });
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
        lvlText.SetText($"Level: {unit.unitData.currentLevel}");

        sourceResource.SetText($"{unit.classData.resourceType}: {unit.unitData.currentResource}/{unit.unitData.maxResource}");
        sourceHealth.SetText($"Health: {unit.unitData.currentHealth}/{unit.unitData.maxHealth}");

        strAttributeNumber.SetText(unit.unitData.strengthStat.ToString());
        dexAttributeNumber.SetText(unit.unitData.dexterityStat.ToString());
        conAttributeNumber.SetText(unit.unitData.constitutionStat.ToString());
        intAttributeNumber.SetText(unit.unitData.intelligenceStat.ToString());
        wisAttributeNumber.SetText(unit.unitData.wisdomStat.ToString());
        chrAttributeNumber.SetText(unit.unitData.charismaStat.ToString());

        hpBar.fillAmount = (float)unit.unitData.currentHealth / unit.unitData.maxHealth;
        sourceHealthBar.fillAmount = (float)unit.unitData.currentHealth / unit.unitData.maxHealth;
        resourceBar.fillAmount = (float)unit.unitData.currentResource / unit.unitData.maxResource;
        sourceResourceBar.fillAmount = (float)unit.unitData.currentResource / unit.unitData.maxResource;

        resourceBar.color = unit.classData.resourceColor;
        sourceResourceBar.color = unit.classData.resourceColor;


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

    public void ShowTargetMenuInfo(Unit Target)
    {
        targetResource.SetText($"{Target.classData.resourceType}: {Target.unitData.currentResource}/{Target.unitData.maxResource}");
        targetHealth.SetText($"Health: {Target.unitData.currentHealth}/{Target.unitData.maxHealth}");
        targetHealthBar.fillAmount = (float)Target.unitData.currentHealth / Target.unitData.maxHealth;
        targetResourceBar.fillAmount = (float)Target.unitData.currentResource / Target.unitData.maxResource;
        targetResourceBar.color = Target.classData.resourceColor;
    }


    private void Update()
    {
        if (statusMenuOpen == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StatusMenuState("down");
                statusMenuOpen = false;
            }
        }

        if (skillMenuOpen == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SkillMenuState("disabled");
                skillMenuOpen = false;
            }
        }
    }

    #endregion
}
