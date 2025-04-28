using UnityEngine;
using System.Collections.Generic;

public class LTacticsMove : MonoBehaviour
{
    List<Tile> selectTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currTile;

    int move;
    int moveSpeed;

    Vector3 velo=new Vector3();
    Vector3 compass = new Vector3();

    float halfHeight=0;

    public Tile TargAdjTile;

    Unit currentUnit;
    Animator currentAnimator;


    public void init(Unit passedUnit, Animator passedAnimator)
    {
        //dev note: move line directly below into top of CompAdjLists() for 
        //manipulated tiles
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        currentUnit = passedUnit;
        currentAnimator = passedAnimator;

        move = currentUnit.unitData.moveDistance;
        moveSpeed = currentUnit.unitData.moveSpeed;


    }

    public void GetCurrTile()
    {
        currTile = GetTargTile(this.gameObject);
        currTile.current = true;
    }

    public Tile GetTargTile(GameObject target)
    {
        RaycastHit hit;
        Tile ttile = null;
        if(Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            ttile = hit.collider.GetComponent<Tile>();
        }
        return ttile;
    }

    public void CompAdjLists(Tile target)
    {
        //place here if applicable
        foreach (GameObject ttile in tiles)
        {
            Tile t = ttile.GetComponent<Tile>();
            t.Neighbors(target);
        }
    }

    public void FindTilesBST()
    {
        GetCurrTile();
        CompAdjLists(currTile); // use the actual tile under the unit

        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currTile);
        currTile.visited=true;

        while(process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectTiles.Add(t);
            t.selectable = true;
            if(t.dist<move)
            {
                foreach (Tile ttile in t.adjList)
                {
                    if(!ttile.visited)
                    {
                        ttile.par = t;
                        ttile.visited=true;
                        ttile.dist = 1 + t.dist;
                        process.Enqueue(ttile);
                    }
                }
            }
        }
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        currentUnit.unitData.isMoving = true;
        currentAnimator.SetBool("isMoving", true);

        Tile next = tile;
        while(next!=null)
        {
            path.Push(next);
            next = next.par;
        }
        //Debug.Log("MoveToTile triggered, path length: " + path.Count);
    }

    public void Move()
    {
        if (path.Count>0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;
            //top of cube calc
            target.y+= halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if(Vector3.Distance(transform.position, target) >= .05f)
            {
                CalcHeading(target);
                SetHorzVel();

                transform.forward=compass;
                transform.position+=velo* Time.deltaTime;
                //Debug.Log("Moving towards: " + target);
            }
            else
            {
                //center reached
                transform.position = target;
                path.Pop();
                //Debug.Log("Arrived at tile: " + target);
            }
        }
        else
        {
            RemoveSelcTiles();
            currentUnit.unitData.isMoving = false;
            currentAnimator.SetBool("isMoving", false);
            //ToDo, move below to action function. later.
            currentUnit.EndTurn();
        }
    }

    void CalcHeading(Vector3 target)
    {
        compass = target - transform.position;
        compass.Normalize();
    }

    void SetHorzVel()
    {
        velo=compass*moveSpeed;
    }

    protected void RemoveSelcTiles()
    {
        if(currTile!=null)
        {
            currTile.current=false;
            // currTile=null;
        }
        foreach (Tile tile in selectTiles)
        {
            tile.Reset();
        }
        selectTiles.Clear();
    }

    protected Tile FindLowestF(List<Tile> listo)
    {
        Tile low = listo[0];
        foreach(Tile t in listo)
        {
            if(t.f < low.f)
            {
                low = t;
            }
        }
        listo.Remove(low);

        return low;
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();
        Tile next = t.par;
        while(next!=null)
        {
            tempPath.Push(next);
            next = next.par;
        }
        if(tempPath.Count <= move)
        {
            return t.par;
        }

        Tile endTile=null;
        for (int i=0;i<=move && tempPath.Count > 0;i++)
        {
            endTile = tempPath.Pop();

        }
        return endTile;
    }

    public void FindPath(Tile target)
    {
        foreach (GameObject tileObj in tiles)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            tile.Reset(); // Make sure this clears: visited, par, g, h, f, etc.
        }
        CompAdjLists(target);
        GetCurrTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currTile);
        currTile.h = Vector3.Distance(currTile.transform.position, target.transform.position);
        currTile.f=currTile.h;

        while (openList.Count > 0)
        {
            Tile t =FindLowestF(openList);
            closedList.Add(t);

            if(t==target)
            {
                TargAdjTile = FindEndTile(t);
                //Debug.Log("Path found, target tile: " + TargAdjTile.name);
                MoveToTile(TargAdjTile);
                return;
            }

            foreach(Tile til in t.adjList)
            {
                if(closedList.Contains(til))
                {
                    //nothing
                }
                else if (openList.Contains(til))
                {
                    float tempG = t.g + Vector3.Distance(til.transform.position, t.transform.position);
                    if(tempG<til.g)
                    {
                        til.par=t;
                        til.g = tempG;
                        til.f = til.g + til.h;
                    }
                }
                else
                {
                    til.par = t;
                    til.g = t.g+ Vector3.Distance(til.transform.position, t.transform.position);
                    til.h = Vector3.Distance(til.transform.position, target.transform.position);
                    til.f=til.g+til.h;

                    openList.Add(til);
                }
            }

        }
        //Debug.LogWarning("Path to target not found.");
        currentUnit.EndTurn();
        //if no path, do stuff here
    }



}
