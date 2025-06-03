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

    private AttackStyle unitAttackStyle;

    [Header("State Machine")]
    public bool hasMoved;
    public bool hasAttack;
    public bool NMEtag = false;
    public bool attack_state;

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

    private GameObject currentRightWeaponInstance;
    private GameObject currentLeftWeaponInstance;


    public UnitState currentState;

    public enum UnitState
    {
        Idle,
        Attack_Check,
        Move_Check,
        Attack_Confirm,
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

        // Highlight attack range
        movementController.FindTilesBST(unitData.attackRange);

        // Wait for player to select a target or press Escape
        while (!unitData.isMoving && attack_state)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Action cancelled by Escape key.");
                cancelledAction = true;
                break;
            }

            CheckMouseAct();
            yield return null;
        }

        if (cancelledAction)
        {
            movementController.RemoveSelcTiles();
            attack_state = false;
            isHandlingAction = false;
            HandleStateTransition(UnitState.Idle);
            yield break;
        }

        // Wait for movement to finish if the action requires it
        while (unitData.isMoving)
        {
            movementController.Move();
            yield return null;
        }

        // Cleanup
        attack_state = false;
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

                CheckMouseMov(); // Your method that checks if player clicked a tile
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

        // Player selected a tile; now wait for movement to complete
        while (unitData.isMoving)
        {
            movementController.Move(); // Movement can't be cancelled once started
            yield return null;
        }

        //unitData.movedThisTurn = true;
        isHandlingMove = false;
        clickcheckM = true;
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
                if (hit.collider.tag == "Tile")
                {
                    Tile targetTile = hit.collider.GetComponent<Tile>();
                    //Debug.Log(t.occupied);
                    if (targetTile.selectable && targetTile.occupied != null)
                    {
                        //print(t.occupied.unitData.name);
                        OnAttack(targetTile.occupied, unitAttackStyle);
                        //HandleStateTransition(UnitState.Attack_Confirm);
                        //clickcheckA = true;
                        //unitData.isMoving = false;
                    }
                }
                // TODO: This needs to check fi its in range
                else if (hit.collider.tag == "Unit")
                {
                    Unit Target = hit.collider.GetComponent<Unit>();
                    if (Target.unitData.Allied == false && Target.unitData.Dead == false)
                        if (AttackDebounce == false)
                        {
                            AttackDebounce = true;
                            OnAttack(Target, unitAttackStyle);
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


    public void OnAttack(Unit target, AttackStyle style)
    {

        StartCoroutine(HandleAttack(target, style));

    }

    public IEnumerator HandleAttack(Unit target, AttackStyle style)
    {

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
            print("second animation");
            yield return new WaitForSeconds(1.6f);
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
            UIManagerScript.UpdateTurnOrderList(TurnOrderScript.ReturnCurrentQueue());
        }

        unitData.attackedThisTurn = true;
        clickcheckA = true;
        HandleStateTransition(UnitState.Idle);
        UIManagerScript.attackButton.interactable = !unitData.attackedThisTurn;
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

            StartCoroutine(UIManagerScript.DisplayToolTip("down", .1f, $"<b>LEFT CLICK</b> on the target again to confirm attack\n\npress <b>ESC</b> to cancel"));
            UIManagerScript.AorMUIState();

        }
    }

    #endregion

    #region Status Effects

    

    #endregion
}
