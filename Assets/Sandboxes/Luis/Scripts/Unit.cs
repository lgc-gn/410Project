using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
/*

UNIT Method Script

Handles player control of units

*/

public class Unit : TacticalUnitBase
{
    public bool dead = false;
    public bool clickcheckM;
    public bool clickcheckA;
    public bool NMEtag = false;
    public bool hasMoved;
    public bool hasAttack;
    public bool attack_state;

    private bool isHandlingMove = false;
    private bool isHandlingAction = false;


    private void Awake()
    {

        movementController = GetComponent<LTacticsMove>();
        animator = GetComponent<Animator>();

        movementController.init(this, animator);



        InitalizeStats();

    }

    public void InitalizeStats()
    {
        unitData.activeTurn = false;
        unitData.currentHealth = unitData.maxHealth;
        unitData.currentResource = unitData.maxResource;
    }

    virtual public void BeginTurn()
    {

        unitData.activeTurn = true;
        clickcheckM = false;
        hasAttack = false;
        hasMoved = false;
        attack_state = false;
        clickcheckA = false;

    }

    public void EndTurn()
    {

        unitData.activeTurn = false;
        unitData.isMoving = false;
        animator.SetBool("isMoving", false);
    }

    public virtual void HandleMoveCommand()
    {
        if (isHandlingMove || hasMoved || !unitData.activeTurn) return;

        StartCoroutine(HandleMoveRoutine());
    }

    public virtual void HandleActionCommand()
    {
        if (isHandlingAction || !unitData.activeTurn || unitData.isMoving)
            return;

        StartCoroutine(HandleActionRoutine());
    }


    #region Action and Movement Coroutines

    private IEnumerator HandleActionRoutine()
    {
        isHandlingAction = true;
        attack_state = true;

        // Highlight attack range
        movementController.FindTilesBST(unitData.attackRange);

        while (!unitData.isMoving && attack_state)
        {
            CheckMouseAct(); // this should check mouse input and set unitData.isMoving = true when clicked
            yield return null;
        }

        // If the attack causes movement (like jumping to a target), wait for it to finish
        while (unitData.isMoving)
        {
            movementController.Move();
            yield return null;
        }

        // Cleanup
        attack_state = false;
        isHandlingAction = false;
        Debug.Log("Action completed.");
    }

    private IEnumerator HandleMoveRoutine()
    {
        isHandlingMove = true;

        if (!unitData.isMoving)
        {
            movementController.FindTilesBST(unitData.moveDistance);

            // Wait until player clicks a valid tile
            while (!unitData.isMoving)
            {
                CheckMouseMov(); // Your method that checks if player clicked a tile
                yield return null; // Wait 1 frame
            }
        }

        // Once isMoving is true, wait for movement to complete
        while (unitData.isMoving)
        {
            movementController.Move();
            yield return null;
        }

        hasMoved = true;
        isHandlingMove = false;
        clickcheckM = true;
        Debug.Log("Unit finished moving.");
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
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if (t.selectable&&t.walk)
                    {
                        movementController.MoveToTile(t);
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
                    Tile t = hit.collider.GetComponent<Tile>();
                    Debug.Log(t.occupied);
                    if (t.selectable && t.occupied != null)
                    {
                        OnAttack(t);
                        Debug.Log("hit success");
                        clickcheckA = true;
                        unitData.isMoving = false;
                    }
                }
            }
        }
    }

    public void OnAttack(Tile t/*Action act = Basic()*/)
    {
        //ToDo: play hit anim
        //ToDo: display damage done
        t.occupied.unitData.currentHealth -= unitData.strengthStat;
        if (t.occupied.unitData.currentHealth <= 0)
        {
            t.occupied.dead = true;
        }
    }

    void Wait()
    {
        unitData.activeTurn = false;
    }

  /*  void CheckAction()
    {
        
    }*/

    #endregion

}
