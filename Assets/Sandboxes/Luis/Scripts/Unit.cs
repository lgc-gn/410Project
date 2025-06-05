using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor.ShaderGraph.Internal;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

/*

UNIT Method Script

Handles player control of units

*/

public class Unit : TacticalUnitBase
{
    [Header("Movement/Action checks")]
    public bool clickcheckM;
    public bool clickcheckA;

    [Header("State Machine")]
    public bool hasMoved;
    public bool hasAttack;
    public bool NMEtag = false;
    public bool attack_state;

    private Unit selectedTarget;
    private AttackStyle unitAttackStyle;

    private bool AttackDebounce;

    [Header("Attach Points")]
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform backTransform;

    [Header("Status Effects")]
    [SerializeField] private List<ActiveStatusEffect> activeEffects = new();


    private bool isHandlingMove = false;
    private bool isHandlingAction = false;

    private UIManager UIManagerScript;
    private TurnOrderHandler TurnOrderScript;
    private Animation legacyAnim;
    private AudioSource audioSource;

    private GameObject currentRightWeaponInstance;
    private GameObject currentLeftWeaponInstance;

    public AudioClip defaultHitSound;

    public UnitState currentState;

    public enum UnitState
    {
        Idle,
        Attack_Check,
        Move_Check,
        Attack_Confirm,
        Move_Confirm,
        Channeling,
        Moving,
        Dead,
        Waiting_For_Input
    }

    public enum AttackStyle
    {
        Standard,
        Monk,
        Rogue
    }

    #region Wake up functions

    private void Awake()
    {
        movementController = GetComponent<LTacticsMove>();
        animator = GetComponent<Animator>();
        movementController.init(this, animator);

        legacyAnim = gameObject.GetComponent<Animation>();
        if (legacyAnim == null)
            legacyAnim = gameObject.AddComponent<Animation>();

        audioSource = gameObject.GetComponent<AudioSource>();

        InitalizeStats();
        InitializeWeapon();

    }

    public void InitalizeStats()
    {
        unitData.Dead = false;
        unitData.activeTurn = false;
        unitData.movedThisTurn = false;
        unitData.currentHealth = unitData.maxHealth;
        unitData.currentResource = unitData.maxResource;
    }

    public void InitializeWeapon()
    {

        if (unitData.RightHand != null)
        {
            WeaponData equippedRightWeapon = unitData.RightHand;

            currentRightWeaponInstance = Instantiate(equippedRightWeapon.weaponModel, rightHandTransform);
            Renderer weaponRenderer = currentRightWeaponInstance.GetComponentInChildren<Renderer>();
            if (equippedRightWeapon.weaponTexture != null)
            {
                weaponRenderer.material.mainTexture = equippedRightWeapon.weaponTexture;

            }

            currentRightWeaponInstance.transform.localPosition = equippedRightWeapon.gripPositionOffset;
            currentRightWeaponInstance.transform.localEulerAngles = equippedRightWeapon.gripRotationOffset;
        }
        if (unitData.LeftHand != null)
        {
            WeaponData equippedLeftWeapon = unitData.LeftHand;

            currentLeftWeaponInstance = Instantiate(equippedLeftWeapon.weaponModel, leftHandTransform);
            Renderer weaponRenderer = currentLeftWeaponInstance.GetComponentInChildren<Renderer>();
            if (equippedLeftWeapon.weaponTexture != null)
            {
                weaponRenderer.material.mainTexture = equippedLeftWeapon.weaponTexture;

            }

            currentLeftWeaponInstance.transform.localPosition = equippedLeftWeapon.gripPositionOffset;
            currentLeftWeaponInstance.transform.localEulerAngles = equippedLeftWeapon.gripRotationOffset;

        }

    }

    //private void Update()
    //{
    //    if (unitData.RightHand != null)
    //    {
    //        WeaponData equippedRightWeapon = unitData.RightHand;
    //        currentRightWeaponInstance.transform.localPosition = equippedRightWeapon.gripPositionOffset;
    //        currentRightWeaponInstance.transform.localEulerAngles = equippedRightWeapon.gripRotationOffset;
    //    }
    //    if (unitData.LeftHand != null)
    //    {
    //        WeaponData equippedLeftWeapon = unitData.RightHand;
    //        currentLeftWeaponInstance.transform.localPosition = equippedLeftWeapon.gripPositionOffset;
    //        currentLeftWeaponInstance.transform.localEulerAngles = equippedLeftWeapon.gripRotationOffset;

    //    }
    //}


    void Start()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            UIManagerScript = gm.GetComponent<UIManager>();
            TurnOrderScript = gm.GetComponent<TurnOrderHandler>();
        }

        if (UIManagerScript == null || TurnOrderScript == null)
        {
            Debug.LogError("UIManager not found on GameManager!");
        }

        if (classData.className == "Monk")
        {
            unitAttackStyle = AttackStyle.Monk;
        }
        else
        {
            unitAttackStyle = AttackStyle.Standard;
        }
    }

    #endregion

    #region Turn Handlers

    public virtual void BeginTurn()
    {

        unitData.activeTurn = true;
        unitData.movedThisTurn = false;
        unitData.attackedThisTurn = false;

        UIManagerScript.moveButton.interactable = !unitData.movedThisTurn;
        UIManagerScript.attackButton.interactable = !unitData.attackedThisTurn;

        selectedTarget = null;
        AttackDebounce = false;

        if (unitData.Allied == true)
            HandleStateTransition(UnitState.Idle);
        else
            UIManagerScript.AorMUIState();

    }

    public virtual void EndTurn()
    {
        unitData.activeTurn = false;
        unitData.isMoving = false;
        animator.SetBool("isMoving", false);

        UIManagerScript.confirmButton.onClick.RemoveAllListeners();
    }
    #endregion

    #region Handlers

    public virtual void HandleMoveCommand()
    {
        if (isHandlingMove || unitData.movedThisTurn == true || !unitData.activeTurn) return;

        StartCoroutine(HandleMoveRoutine());
    }

    public virtual void HandleActionCommand()
    {
        if (isHandlingAction || !unitData.activeTurn || unitData.isMoving)
            return;

        StartCoroutine(HandleActionRoutine());
    }

    #endregion

    #region Action and Movement Coroutines

    private IEnumerator HandleActionRoutine()
    {
        isHandlingAction = true;
        attack_state = true;
        bool cancelledAction = false;
        bool waitingForConfirm = false;

        movementController.FindTilesBST(unitData.attackRange);

        while (!unitData.isMoving && attack_state)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cancelledAction = true;
                break;
            }

            if (!waitingForConfirm)
            {
                CheckMouseAct();

                if (selectedTarget != null)
                {

                    waitingForConfirm = true;
                }
            }

            yield return null;
        }

        if (cancelledAction)
        {
            movementController.RemoveSelcTiles();
            attack_state = false;
            isHandlingAction = false;
            selectedTarget = null;
            HandleStateTransition(UnitState.Idle);
            yield break;
        }

        while (waitingForConfirm)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedTarget = null;
                HandleStateTransition(UnitState.Idle);
                waitingForConfirm = false;
            }

            if (!attack_state)
            {
                waitingForConfirm = false;
            }

            yield return null;
        }

        isHandlingAction = false;
    }



    private IEnumerator HandleMoveRoutine()
    {
        isHandlingMove = true;
        bool cancelledMove = false;

        if (!unitData.isMoving)
        {
            movementController.FindTilesBST(unitData.moveDistance);

            // Wait until player clicks a valid tile or presses Escape
            while (!unitData.isMoving)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    //Debug.Log("Movement cancelled before starting.");
                    cancelledMove = true;
                    
                    break;
                }

                CheckMouseMov(); 
                yield return null;
            }
        }

        if (cancelledMove)
        {
            movementController.RemoveSelcTiles();
            isHandlingMove = false;
            clickcheckM = false;
            HandleStateTransition(UnitState.Idle);
            yield break;
        }

        while (unitData.isMoving)
        {
            movementController.Move();
            yield return null;
        }

        isHandlingMove = false;
        clickcheckM = true;
        unitData.movedThisTurn = true;
        HandleStateTransition(UnitState.Idle);
        UIManagerScript.moveButton.interactable = !unitData.movedThisTurn;
    }


    #endregion

    #region Helper Functions


    void CheckMouseMov()
    {

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if (t.selectable&&t.walk)
                    {
                        movementController.MoveToTile(t);
                        UIManagerScript.NoToolTipState();

                    }
                }
            }
        }
    }

    //needs to check if selected tile has a unit
    void CheckMouseAct()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.CompareTag("Tile"))
                {
                    Tile targetTile = hit.collider.GetComponent<Tile>();

                    if (targetTile.selectable && targetTile.occupied != null)
                    {
                        selectedTarget = targetTile.occupied;
                        //Debug.Log(selectedTarget.unitData.characterName);
                        HandleStateTransition(UnitState.Attack_Confirm);
                    }
                }

                else if (hit.collider.CompareTag("Unit"))
                {
                    Unit raycastTarget = hit.collider.GetComponent<Unit>();

                    // Only allow attacks on alive enemies
                    if (!raycastTarget.unitData.Allied && !raycastTarget.unitData.Dead)
                    {
                        // Raycast straight down from slightly above the unit's feet
                        Vector3 rayStart = raycastTarget.transform.position + Vector3.up * 0.1f;
                        Ray downRay = new Ray(rayStart, Vector3.down);

                        if (Physics.Raycast(downRay, out RaycastHit tileHit, 2f))
                        {
                            Tile tileBelow = tileHit.collider.GetComponent<Tile>();

                            if (tileBelow != null)
                            {
                                if (tileBelow.selectable || tileBelow.attackable)
                                {
                                    selectedTarget = raycastTarget;
                                    HandleStateTransition(UnitState.Attack_Confirm);
                                }
                                else
                                {
                                    Debug.Log("Tile under unit is not marked selectable or attackable.");
                                }
                            }
                            else
                            {
                                Debug.Log("Raycast hit something, but it wasn�t a tile.");
                            }
                        }
                        else
                        {
                            Debug.Log("No tile found below unit.");
                        }
                    }
                    else
                    {
                        Debug.Log("Clicked on ally or dead unit");
                    }
                }

            }
        }
    }


    IEnumerator RotateToTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 13f);
            yield return null;
        }

        transform.rotation = targetRot; 
    }


    #endregion

    #region Combat

    public void OnAttack(Unit target, AttackStyle style)
    {
        //print($"Attacking {target.unitData.characterName}");
        StartCoroutine(HandleAttack(target, style));

    }

    public void PlaySkillSFX(SkillData skillToCast)
    {
        if (skillToCast != null && skillToCast.hitSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.PlayOneShot(skillToCast.hitSound);
            }
            else
            {
                Debug.LogWarning("No AudioSource found on unit for playing skill sound.");
            }
        }
    }

    public void OnSkillSelected(SkillData skill)
    {
        print(skill.skillName);
        StartCoroutine(HandleSkill(null, skill));
    }


    public IEnumerator HandleAttack(Unit target, AttackStyle style)
    {
        //maybe try adding the aoe check somewhere near the start and if AOE true, call attackhoverdetection to have a working
        //aoe marker? not entirely sure but if it were to be called, it would be in here
        UIManagerScript.NoToolTipState();

        yield return StartCoroutine(RotateToTarget(target.transform));

        animator.Play("Attack");

        Vector3 MidPoint = (rightHandTransform.position + target.transform.position) * 0.5f;

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        if (style == AttackStyle.Rogue)
            yield return new WaitForSeconds(animationLength * 0.7f);
        else
            yield return new WaitForSeconds(animationLength * 0.4f);


        float DamageValue = unitData.RightHand.baseDamage;
        target.unitData.currentHealth -= DamageValue;

        UIManagerScript.DisplayDamageNumber(DamageValue, target);
        AudioSource.PlayClipAtPoint(defaultHitSound, target.transform.position);

        if (unitData.RightHand.OnHitParticle != null)
        {
            GameObject effectInstance = Instantiate(unitData.RightHand.OnHitParticle, MidPoint, Quaternion.identity);
            ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
            ps.Play();
            Destroy(effectInstance, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        target.animator.Play("HitReaction");

        #region Monk specific
        if (style == AttackStyle.Monk)
        {
            yield return new WaitForSeconds(.7f);
            float DamageValue2 = unitData.LeftHand.baseDamage + 1.0f;
            target.unitData.currentHealth -= DamageValue2;
            target.animator.Play("HitReaction", 0, 0f);

            MidPoint = (leftHandTransform.position + target.transform.position) * 0.5f;

            if (unitData.LeftHand.OnHitParticle != null)
            {
                GameObject effectInstance = Instantiate(unitData.RightHand.OnHitParticle, MidPoint, Quaternion.identity);
                ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
                ps.Play();
                Destroy(effectInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }

            UIManagerScript.DisplayDamageNumber(DamageValue2, target);
        }
        #endregion

        #region Rogue specific
        if (style == AttackStyle.Rogue)
        {
            //print("second animation");
            yield return new WaitForSeconds(1.4f);
            float DamageValue2 = unitData.LeftHand.baseDamage + 1.0f;
            target.unitData.currentHealth -= DamageValue2;
            target.animator.Play("HitReaction", 0, 0f);

            MidPoint = (leftHandTransform.position + target.transform.position) * 0.5f;

            if (unitData.LeftHand.OnHitParticle != null)
            {
                GameObject effectInstance = Instantiate(unitData.RightHand.OnHitParticle, MidPoint, Quaternion.identity);
                ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
                ps.Play();
                Destroy(effectInstance, ps.main.duration + ps.main.startLifetime.constantMax);
            }

            UIManagerScript.DisplayDamageNumber(DamageValue2, target);
        }
        #endregion

        if (target.unitData.currentHealth <= 0f)
        {
            target.unitData.Dead = true;
            target.animator.Play("Death");
            TurnOrderScript.RefreshQueue();
            TurnOrderScript.CheckWinConditions();
            Tile tileUnderneath = null;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
            {
                tileUnderneath = hit.collider.GetComponent<Tile>();
                if (tileUnderneath != null && tileUnderneath.occupied == this)
                {
                    //tileUnderneath.occupied = null;
                }
            }
            UIManagerScript.UpdateTurnOrderList(TurnOrderScript.ReturnCurrentQueue());
        }

        unitData.attackedThisTurn = true;
        clickcheckA = true;
        HandleStateTransition(UnitState.Idle);
        UIManagerScript.attackButton.interactable = !unitData.attackedThisTurn;
    }

    public IEnumerator HandleSkill(Unit initialTarget, SkillData skillToCast)
    {
        UIManagerScript.SkillMenuState("disabled");
        bool confirmed = false;
        bool waitingForConfirm = true;
        attack_state = true;

        Unit target = null;
        bool cancelled = false;

        if (skillToCast.skillType == SkillData.SkillType.Self)
        {
            target = this;

            if (skillToCast.channelAnim != null)
            {
                animator.Play("Channel");
                yield return new WaitForSeconds(skillToCast.channelAnim.length);
            }

            UIManagerScript.unitInfoPanel.SetActive(false);
            UIManagerScript.targetCombatPanel.SetActive(true);
            UIManagerScript.confirmButtonPanel.SetActive(true);

            UIManagerScript.ShowTargetMenuInfo(skillToCast.skillType == SkillData.SkillType.Self ? this : target);

            StartCoroutine(UIManagerScript.DisplayToolTip("down", 0.1f,
                $"Cast <b>{skillToCast.skillName}</b>?\n\n<color=yellow>ESC</color> to cancel"));

            UIManagerScript.confirmButton.onClick.RemoveAllListeners();
            UIManagerScript.confirmButton.onClick.AddListener(() =>
            {
                confirmed = true;
                waitingForConfirm = false;
                UIManagerScript.NoToolTipState();
            });

            while (waitingForConfirm)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HandleStateTransition(UnitState.Idle);
                    attack_state = false;
                    yield break;
                }

                yield return null;
            }

            UIManagerScript.confirmButtonPanel.SetActive(false);
            UIManagerScript.targetCombatPanel.SetActive(false);
            UIManagerScript.unitInfoPanel.SetActive(true);
            UIManagerScript.NoToolTipState();
        }
        else
        {

            movementController.FindTilesBST(skillToCast.skillRange);
            StartCoroutine(UIManagerScript.DisplayToolTip("down", 0.1f, $"Click a target for <b>{skillToCast.skillName}</b>\n\n<color=yellow>ESC</color> to cancel"));

            // Wait for valid target selection
            while (target == null)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cancelled = true;
                    attack_state = false;
                    break;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.CompareTag("Unit"))
                        {
                            Unit u = hit.collider.GetComponent<Unit>();
                            if (u != null && !u.unitData.Dead)
                            {
                                // Raycast down from slightly above the unit to find the tile below
                                Vector3 rayStart = u.transform.position + Vector3.up * 0.1f;
                                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit tileHit, 2f))
                                {
                                    Tile tileBelow = tileHit.collider.GetComponent<Tile>();
                                    if (tileBelow != null && (tileBelow.selectable || tileBelow.attackable))
                                    {
                                        target = u;
                                        break;
                                    }
                                    else
                                    {
                                        Debug.Log("Tile under unit is not selectable or attackable.");
                                    }
                                }
                                else
                                {
                                    Debug.Log("No tile found below the unit.");
                                }
                            }
                        }


                        else if (skillToCast.skillType == SkillData.SkillType.Area_of_Effect && hit.collider.CompareTag("Tile"))
                        {
                            // Optional: For AOE support by tile � placeholder
                            Tile targetTile = hit.collider.GetComponent<Tile>();
                            if (targetTile.selectable)
                            {
                                // You could collect affected units here
                                break;
                            }
                        }
                    }
                }

                yield return null;
            }

            if (cancelled)
            {
                HandleStateTransition(UnitState.Idle);
                yield break;
            }
        }


        yield return StartCoroutine(RotateToTarget(target.transform));

        if (skillToCast.startupAnim != null){
            animator.Play(skillToCast.startupAnim.name);
            yield return new WaitForSeconds(skillToCast.startupAnim.length);
        }

        if (skillToCast.channelAnim != null) {
            animator.Play(skillToCast.channelAnim.name, 0, 0f);
        }

        UIManagerScript.unitInfoPanel.SetActive(false);
        UIManagerScript.targetCombatPanel.SetActive(true);
        UIManagerScript.confirmButtonPanel.SetActive(true);
        UIManagerScript.ShowTargetMenuInfo(target);
        StartCoroutine(UIManagerScript.DisplayToolTip("down", 0.1f, $"Cast <b>{skillToCast.skillName}?</b>\n\n<color=yellow>ESC</color> to cancel"));

        UIManagerScript.confirmButton.onClick.RemoveAllListeners();

        UIManagerScript.confirmButton.onClick.AddListener(() =>
        {
            confirmed = true;
            waitingForConfirm = false;
        });

        while (waitingForConfirm)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                animator.Play("Idle");
                HandleStateTransition(UnitState.Idle);
                attack_state = false;
                yield break;
            }

            yield return null;
        }

        UIManagerScript.confirmButtonPanel.SetActive(false);
        UIManagerScript.targetCombatPanel.SetActive(false);
        UIManagerScript.unitInfoPanel.SetActive(true);

        if (skillToCast.castAnim != null){
            animator.Play(skillToCast.castAnim.name);
            yield return new WaitForSeconds(skillToCast.castAnim.length * skillToCast.impactMultiplier);
        }
        AudioSource.PlayClipAtPoint(skillToCast.hitSound, transform.position);
        PlayHitEffect(skillToCast, target.transform.position);

        if (skillToCast.skillType != SkillData.SkillType.Area_of_Effect && target != null)
        {
            float value = skillToCast.skillDamage;

            switch (skillToCast.resourceType)
            {
                case SkillData.AfflictedResource.Damage:
                    target.unitData.currentHealth -= value;
                    target.animator.Play("HitReaction");
                    UIManagerScript.DisplayDamageNumber(value, target);
                    break;

                case SkillData.AfflictedResource.Heal:
                    target.unitData.currentHealth = Mathf.Min(target.unitData.maxHealth, target.unitData.currentHealth + value);
                    UIManagerScript.DisplayDamageNumber(-value, target);
                    break;

                case SkillData.AfflictedResource.Mana:
                    target.unitData.currentResource = Mathf.Min(target.unitData.maxResource, target.unitData.currentResource + value);
                    break;
            }

            if (skillToCast.statusEffect != null)
            {
                // Apply a status effect (you'd need your own handler logic)
                //target.AddStatusEffect(skillToCast.statusEffect);
            }

            // Death check
            if (target.unitData.currentHealth <= 0)
            {
                target.unitData.Dead = true;
                target.animator.Play("Death");
                TurnOrderScript.RefreshQueue();
                TurnOrderScript.CheckWinConditions();
                Tile tileUnder = null;
                if (Physics.Raycast(target.transform.position, Vector3.down, out RaycastHit hit, 1f))
                {
                    tileUnder = hit.collider.GetComponent<Tile>();
                    if (tileUnder != null && tileUnder.occupied == target)
                        //tileUnder.occupied = null;
                        print("Dead body on tile");
                }

                UIManagerScript.UpdateTurnOrderList(TurnOrderScript.ReturnCurrentQueue());
            }
        }

        //if (skillToCast.freeAction == false)
        //{
        //    print("Not free action");
        //    unitData.attackedThisTurn = true;
        //    UIManagerScript.attackButton.interactable = !unitData.attackedThisTurn;
        //}

        attack_state = false;
        clickcheckA = true;
        unitData.attackedThisTurn = true;
        UIManagerScript.attackButton.interactable = !unitData.attackedThisTurn;
        unitData.currentResource -= skillToCast.resourceChange;
        UIManagerScript.ShowUnitInfo(TurnOrderScript.ReturnCurrentQueue().Peek());
        HandleStateTransition(UnitState.Idle);
    }

    public void AddStatusEffect(StatusEffectData data)
    {
        if (data == null) return;
        ActiveStatusEffect newEffect = new ActiveStatusEffect(data);
        activeEffects.Add(newEffect);
        Debug.Log($"{unitData.characterName} gained status: {data.statusName}");
    }

    public void PlayHitEffect(SkillData skill, Vector3 point)
    {
        if (skill.hitEffect == null)
        {
            Debug.LogWarning($"No hit effect assigned in skill: {skill.skillName}");
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 1.5f;
        ParticleSystem instance = Instantiate(skill.hitEffect, point, Quaternion.identity);
        instance.Play();

        float lifetime = instance.main.duration + instance.main.startLifetime.constantMax;
        Destroy(instance.gameObject, lifetime);
    }



    #endregion

    #region State Machine

    public virtual void HandleStateTransition(UnitState newState)
    {

        if (newState == UnitState.Idle)
        {
            currentState = newState;

            AttackDebounce = false;
            movementController.RemoveSelcTiles();

            UIManagerScript.IdleUIState();
            clickcheckM = false;
            attack_state = false;
            clickcheckA = false;

            UIManagerScript.unitInfoPanel.SetActive(true);
            UIManagerScript.targetCombatPanel.SetActive(false);
            UIManagerScript.confirmButtonPanel.SetActive(false);

        }
        else if (newState == UnitState.Attack_Check)
        {

            if (unitData.attackedThisTurn == true)
            {
                return;
            }

            currentState = newState;

            StartCoroutine(UIManagerScript.DisplayToolTip("down", .1f, $"<b>LEFT CLICK</b> on any <b><color=red>RED</color></b> tile {unitData.name} should attack towards\n\npress <b>ESC</b> to cancel"));
            UIManagerScript.AorMUIState();
            HandleActionCommand();
        }
        else if (newState == UnitState.Move_Check)
        {
            if (unitData.movedThisTurn == true)
            {
                return;
            }

            currentState = newState;

            StartCoroutine(UIManagerScript.DisplayToolTip("down", .1f, $"<b>LEFT CLICK</b> on any <b><color=#43A7CC>BLUE</color></b> tile {unitData.name} should move towards\n\npress <b>ESC</b> to cancel"));
            UIManagerScript.AorMUIState();
            HandleMoveCommand();
        }
        else if (newState == UnitState.Attack_Confirm)
        {

            if (unitData.attackedThisTurn == true)
            {
                return;
            }

            StartCoroutine(UIManagerScript.DisplayToolTip("down", .1f, $"Confirm your attack on <b>{selectedTarget.unitData.characterName}</b>?\n\npress <b>ESC</b> to cancel"));
            UIManagerScript.AorMUIState();
            UIManagerScript.unitInfoPanel.SetActive(false);
            UIManagerScript.targetCombatPanel.SetActive(true);

            UIManagerScript.confirmButtonPanel.SetActive(true);
            UIManagerScript.ShowTargetMenuInfo(selectedTarget);

            UIManagerScript.confirmButton.onClick.RemoveAllListeners();

            UIManagerScript.confirmButton.onClick.AddListener(() => {
                if (AttackDebounce == false)
                {
                    OnAttack(selectedTarget, unitAttackStyle);
                    AttackDebounce = true;
                }
            });


        }

        else if (newState == UnitState.Move_Confirm)
        {
            if (unitData.attackedThisTurn == true)
            {
                return;
            }

            StartCoroutine(UIManagerScript.DisplayToolTip("down", .1f, $"<b>CONFIRM</b> on the target again to confirm attack\n\npress <b>ESC</b> to cancel"));
            UIManagerScript.AorMUIState();
            UIManagerScript.unitInfoPanel.SetActive(false);
            UIManagerScript.targetCombatPanel.SetActive(true);

        }
    }

    #endregion

    #region Status Effects

    

    #endregion
}
