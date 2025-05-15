using System.Collections;
using UnityEngine;

/*
 Handles enemy AI, derives from TacticalUnitBase
 */

public class EnemyUnit : Unit
{
    public bool end=false;
    GameObject target2;

    private void Awake()
    {

        movementController = GetComponent<LTacticsMove>();
        animator = GetComponent<Animator>();

        movementController.init(this, animator);

        InitalizeStats();
        NMEtag=true;
    }


    override public void HandleMoveCommand()
    {
        clickcheckM=true;
        if(clickcheckM && hasMoved==false)
        {

            if (unitData.activeTurn == false)
            {
                return;
            } 

            if (!unitData.isMoving)
            {
                if (target2 == null)
                {
                    //Debug.Log("Finding target...");
                    FindNearTarg(); 
                }

                //Debug.Log("Attempting to calculate path...");
                CalcPath(); 

                if (movementController.TargAdjTile != null)
                {
                    movementController.TargAdjTile.target = true;
                    //Debug.Log("Target tile found: " + movementController.TargAdjTile.name);
                }
                else
                {
                    //Debug.LogWarning("TargAdjTile was null after pathfinding. Ending turn.");
                    this.EndTurn(); 
                }
            }
            else
            {
                movementController.Move();   
            }
        }
    }

    void Update()
    {
        HandleMoveCommand();
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

    void FindNearTarg()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Unit");
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            if (obj.TryGetComponent<Unit>(out var unitComponent)
                && !obj.TryGetComponent<EnemyUnit>(out _))
            {
                float d = Vector3.Distance(transform.position, obj.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = obj;
                }
            }
        }

        target2 = nearest;
        //Debug.Log("Target found: " + (target2 != null ? target2.name : "None"));
    }


    #endregion
}
