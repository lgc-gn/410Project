using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

UNIT Method Script

Pulls data from inputted UnitData scriptable object



*/

public class Unit : TacticalUnitBase
{
    // Update is called once per frame
    void Update()
    {
        //attack updates here. prio over turn
        if(!turn)
        {
            return;
        }
        if (!moving)
        {
            FindTilesBST();
            CheckMouse();
        }
        else
        {
            Move();
        }

    }

    void CheckMouse()
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
                            //move code
                            MoveToTile(t);
                        }
                    }
                }
            }
    }



}
