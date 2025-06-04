using UnityEngine;
using System.Collections.Generic;

public class LTacticsMove : MonoBehaviour
{
    public List<Tile> selectTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currTile;

    int move;
    int moveSpeed;

    Vector3 velo = new Vector3();
    Vector3 compass = new Vector3();
    public List<Tile> Movable = new List<Tile>();
    public List<Tile> Attackable = new List<Tile>();

    float halfHeight = 0;

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

        RaycastHit tiler;
        if (Physics.Raycast(currentUnit.transform.position, Vector3.down, out tiler, 1))
        {
            currTile = tiler.collider.GetComponent<Tile>();
        }

        move = currentUnit.unitData.moveDistance;
        moveSpeed = currentUnit.unitData.moveSpeed;


    }

    public void GetCurrTile()
    {
        currTile.occupied = null;
        currTile = GetTargTile(this.gameObject);
        currTile.current = true;
    }

    public Tile GetTargTile(GameObject target)
    {
        RaycastHit hit;
        Tile ttile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
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
            t.attackstate = currentUnit.attack_state;
            t.Neighbors(target);
        }
    }

    public void FindTilesBST(int range)
    {
        // Prepare
        GetCurrTile();
        CompAdjLists(currTile); // builds adjacency for all tiles

        // Reset lists
        selectTiles.Clear();
        Movable.Clear();
        Attackable.Clear();

        Queue<Tile> process = new Queue<Tile>();
        currTile.visited = true;
        process.Enqueue(currTile);

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();
            selectTiles.Add(t);  // All reachable tiles for this range
            t.selectable = true;

            // Categorize
            if (t.occupied == null || t.occupied.unitData.Dead)
            {
                Movable.Add(t);
            }
            else
            {
                Attackable.Add(t);
            }
            //t.selectable = true;

            // Expand neighbors if within range
            if (t.dist < range)
            {
                foreach (Tile neighbor in t.adjList)
                {
                    if (!neighbor.visited)
                    {
                        neighbor.visited = true;
                        neighbor.par = t;
                        neighbor.dist = t.dist + 1;
                        process.Enqueue(neighbor);
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
        while (next != null)
        {
            path.Push(next);
            next = next.par;
        }
        //Debug.Log("MoveToTile triggered, path length: " + path.Count);
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;
            //top of cube calc
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (t.occupied != null && t.occupied.NMEtag && !t.occupied.unitData.Dead)
            {
                Debug.LogWarning("Blocked by enemy unit on tile: " + t.name);
                currentUnit.unitData.isMoving = false;
                currentAnimator.SetBool("isMoving", false);
                currentUnit.clickcheckM = false;
                currentUnit.hasMoved = true;
                return;
            }

            if (Vector3.Distance(transform.position, target) >= .05f)
            {
                CalcHeading(target);
                SetHorzVel();

                transform.forward = compass;
                transform.position += velo * Time.deltaTime;
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
            currentUnit.clickcheckM = false;
            currentUnit.hasMoved = true;
            currTile.occupied = currentUnit.GetComponent<Unit>();
            if (currentUnit.NMEtag)
            {
                EnemyUnit atkNME = currentUnit.GetComponent<EnemyUnit>();
                atkNME.attack_state = true;
                atkNME.FindTilesBST(atkNME.unitData.attackRange);
                //Debug.Log("hey wtf");
                atkNME.Attacking();
            }
        }
    }

    void CalcHeading(Vector3 target)
    {
        compass = target - transform.position;
        compass.Normalize();
    }

    void SetHorzVel()
    {
        velo = compass * moveSpeed;
    }

    public void RemoveSelcTiles()
    {
        if (currTile != null)
        {
            currTile.current = false;
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
        foreach (Tile t in listo)
        {
            if (t.f < low.f)
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
        while (next != null)
        {
            tempPath.Push(next);
            next = next.par;
        }
        if (tempPath.Count <= move)
        {
            return t.par;
        }

        Tile endTile = null;
        for (int i = 0; i <= move && tempPath.Count > 0; i++)
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
            tile.Reset(); // Clear visited, parent, cost values
        }

        CompAdjLists(target);
        GetCurrTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currTile);
        currTile.h = Vector3.Distance(currTile.transform.position, target.transform.position);
        currTile.f = currTile.h;

        while (openList.Count > 0)
        {
            Tile t = FindLowestF(openList);
            closedList.Add(t);

            if (t == target)
            {
                TargAdjTile = FindEndTile(t);
                MoveToTile(TargAdjTile);
                return;
            }

            foreach (Tile til in t.adjList)
            {
                //  NEW: Skip unwalkable tiles
                if (!til.walk)
                    continue;

                if (closedList.Contains(til))
                {
                    continue;
                }
                else if (openList.Contains(til))
                {
                    float tempG = t.g + Vector3.Distance(til.transform.position, t.transform.position);
                    if (tempG < til.g)
                    {
                        til.par = t;
                        til.g = tempG;
                        til.f = til.g + til.h;
                    }
                }
                else
                {
                    if (til.occupied != null && til.occupied.NMEtag && !til.occupied.unitData.Dead)
                    {
                        continue;
                    }

                    til.par = t;
                    til.g = t.g + Vector3.Distance(til.transform.position, t.transform.position);
                    til.h = Vector3.Distance(til.transform.position, target.transform.position);
                    til.f = til.g + til.h;

                    openList.Add(til);
                }
            }
        }

        // If no path found, try new target
        RaycastHit hit;
        Unit uni = null;
        if (Physics.Raycast(target.transform.position, Vector3.up, out hit, 1))
        {
            uni = hit.collider.GetComponent<Unit>();
        }

        this.GetComponent<EnemyUnit>().toIgnore.Add(uni);
        this.GetComponent<EnemyUnit>().FindNearTarg();
    }
}

