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

    //public IEnumerator HandleMoveRoutine()
    //{
    //    if (!unitData.isMoving)
    //    {
    //        movementController.FindTilesBST(unitData.moveDistance);

    //        while (!unitData.isMoving)
    //        {
    //            CheckMouseMov();

    //            // Cancel if Escape is pressed
    //            if (Input.GetKeyDown(KeyCode.Escape))
    //            {
    //                movementController.RemoveSelcTiles();
    //                yield break;
    //            }

    //            yield return null; // Wait for next frame
    //        }
    //    }

    //    print("moving");
    //    movementController.Move();
    //}


    //adjusted check for attack range
    /*public virtual void HandleActionCommand()
    {
        if (!unitData.activeTurn)
        {
            return;
        }

        if (!unitData.isMoving)
        {
            movementController.FindTilesBST(unitData.range);
            CheckMouseAct();
        }
    }*/

    void Update()
    {

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
 /*   void CheckMouseAct()
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
                        
                    }
                }
            }
        }
    } */

    void Wait()
    {
        unitData.activeTurn = false;
    }

  /*  void CheckAction()
    {
        
    }*/

    #endregion

}
