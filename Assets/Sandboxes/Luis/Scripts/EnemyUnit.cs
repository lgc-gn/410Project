using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/*
 Handles enemy AI, derives from TacticalUnitBase
 */

public class EnemyUnit : Unit
{
    public bool end = false;
    GameObject target2;
    public List<Unit> toIgnore = new List<Unit>();

    private AttackStyle enemyAttackStyle;

    private bool isReadyToAttack = false;
    private bool hasAttacked = false;
    private bool isMovingToTarget = false;
    private bool isAttacking = false;
    private bool hasCompletedTurn = false;


    private void Awake()
    {

        movementController = GetComponent<LTacticsMove>();
        animator = GetComponent<Animator>();

        movementController.init(this, animator);

        InitalizeStats();
        InitializeWeapon();
        NMEtag = true;

        if (classData.className == "Monk")
            enemyAttackStyle = AttackStyle.Monk;
        else if (classData.className == "Rogue")
            enemyAttackStyle = AttackStyle.Rogue;
        else
            enemyAttackStyle = AttackStyle.Standard;
    }

    public override void BeginTurn()
    {
        // Reset Enemy-specific control flags
        isMovingToTarget = false;
        isAttacking = false;
        hasCompletedTurn = false;
        target2 = null;

        // Call base BeginTurn to reset unitData flags and UI
        base.BeginTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();

        // Extra cleanup for enemy AI (optional)
        isMovingToTarget = false;
        isAttacking = false;
        hasCompletedTurn = true;
        target2 = null;
    }



    override public void HandleMoveCommand()
    {
        if (unitData.activeTurn == false || unitData.movedThisTurn || hasCompletedTurn)
            return;

        if (!isMovingToTarget && target2 == null)
        {
            FindNearTarg();
            if (target2 == null)
            {
                EndTurn();
                hasCompletedTurn = true;
                return;
            }

            Tile targetTile = movementController.GetTargTile(target2);
            if (targetTile == null)
            {
                EndTurn();
                hasCompletedTurn = true;
                return;
            }

            movementController.FindPath(targetTile);
            if (movementController.TargAdjTile != null)
            {
                movementController.TargAdjTile.target = true;
                isMovingToTarget = true;
            }
            else
            {
                EndTurn();
                hasCompletedTurn = true;
            }
        }

        // Movement phase
        if (isMovingToTarget && target2 && !target2.GetComponent<Unit>().unitData.Dead)
        {
            if (!unitData.isMoving)
            {
                isMovingToTarget = false;
                isAttacking = true; // Move finished, prepare to attack
            }
            else
            {
                movementController.Move(); // Continue moving
            }
        }
    }


    override public void HandleStateTransition(UnitState newState)
    {
        print("asd");
    }

    void Update()
    {
        if (!hasCompletedTurn)
        {
            HandleMoveCommand();
        }

        if (isAttacking && !hasCompletedTurn)
        {
            isAttacking = false;
            StartCoroutine(Attacking());
        }
    }



    #region Helper Functions

    void CalcPath()
    {
        Tile targetTile = movementController.GetTargTile(target2);
        if (targetTile == null)
        {
            //Debug.LogWarning("Target tile not found! Ending turn.");
            this.EndTurn();
            return;
        }

        movementController.FindPath(targetTile); // Find the path to the target tile.

        if (movementController.TargAdjTile == null)
        {
            //Debug.LogWarning("No path to target! Ending turn.");
            this.EndTurn();
        }
    }

    public void FindNearTarg()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Unit");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            if (obj.TryGetComponent<Unit>(out var unitComponent)
                && !obj.TryGetComponent<EnemyUnit>(out _))
            {
                Unit objUni = obj.GetComponent<Unit>();
                if (!toIgnore.Contains(objUni)&&!objUni.unitData.Dead)
                {
                    float d = Vector3.Distance(transform.position, obj.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearest = obj;
                    }
                }
            }
        }

        target2 = nearest;
        Debug.Log("Target found: " + (target2 != null ? target2.name : "None"));
    }

    public IEnumerator Attacking()
    {

        foreach (Tile tili in selectTiles)
        {
            if (tili.occupied && !tili.occupied.NMEtag)
            {
                yield return StartCoroutine(EnemyAttack(tili.occupied, enemyAttackStyle));
                EndTurn();
                hasCompletedTurn = true;
                yield break;
            }
        }

        EndTurn();
        hasCompletedTurn = true;
    }


    public IEnumerator EnemyAttack(Unit target, AttackStyle style)
    {

        yield return StartCoroutine(HandleAttack(target, style));

        yield return new WaitForSecondsRealtime(2f);

    }



    #endregion
}
