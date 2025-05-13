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
    public bool clickcheck;
    public bool NMEtag=false;
    public bool hasMoved;
    public bool hasAttack;
    public bool attack_state;


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

    public void BeginTurn()
    {

        unitData.activeTurn = true;
        clickcheck=false;
        hasAttack=false;
        hasMoved=false;

    }

    public void EndTurn()
    {

        unitData.activeTurn = false;
        unitData.isMoving = false;
        animator.SetBool("isMoving", false);

        // Add additional logic if needed to reset other state variables
    }
    
    public virtual void HandleMoveCommand()
    {
        if(clickcheck&&hasMoved==false)
        {
            if (!unitData.activeTurn)
            {
                return;
            }

            if (!unitData.isMoving)
            {
                movementController.FindTilesBST(unitData.moveDistance);
                CheckMouseMov();
            }

            else
            {
                movementController.Move();
            }
        }
    }

    //adjusted check for attack range
    public virtual void HandleActionCommand()
    {
        if (!unitData.activeTurn)
        {
            return;
        }

        if (!unitData.isMoving)
        {
            movementController.FindTilesBST(1);
            CheckMouseAct();
        }
    }


    void Update()
    {
        if(Input.GetKey(KeyCode.F))
        {
            attack_state=true;
            clickcheck=false;
            HandleActionCommand();
        }
        if(Input.GetKey(KeyCode.A))
        {
            clickcheck=true;
        }
        HandleMoveCommand();
    }

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
                    if (t.selectable)
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
                    if (t.selectable&&t.occupied!=null)
                    {
                        Debug.Log("hit success");
                        Wait();
                    }
                }
            }
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
