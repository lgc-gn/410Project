using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NME : TacticalUnitBase
{
    GameObject target2;
    // Update is called once per frame
    void Update()
    {
        Debug.Log("NME Update is running");
        if (!turn)
        {
            return;
        } // If it's not the enemy's turn, do nothing.

        if (!moving)
        {
            if (target2 == null)
            {
                Debug.Log("Finding target...");
                FindNearTarg(); // Find a new target if there is none.
            }

            Debug.Log("Attempting to calculate path...");
            CalcPath(); // Try to calculate the path to the target.

            if (TargAdjTile != null)
            {
                TargAdjTile.target = true;
                Debug.Log("Target tile found: " + TargAdjTile.name);
            }
            else
            {
                Debug.LogWarning("TargAdjTile was null after pathfinding. Ending turn.");
                EndTurn(); // End the turn if no path is found.
            }
        }
        else
        {
            Debug.Log("Moving...");
            Move(); // Move towards the target if moving is true.
        }
    }

    void CalcPath()
    {
        Tile targetTile = GetTargTile(target2);
        if (targetTile == null)
        {
            Debug.LogWarning("Target tile not found! Ending turn.");
            EndTurn();
            return;
        }

        FindPath(targetTile); // Find the path to the target tile.

        if (TargAdjTile == null)
        {
            Debug.LogWarning("No path to target! Ending turn.");
            EndTurn(); // End the turn if no path exists.
        }
    }

    void FindNearTarg()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Unit");
        GameObject nearest = null;
        float distan = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if (d < distan)
            {
                distan = d;
                nearest = obj;
            }
        }
        target2 = nearest;
        Debug.Log("Target found: " + (target2 != null ? target2.name : "None"));
    }

}
