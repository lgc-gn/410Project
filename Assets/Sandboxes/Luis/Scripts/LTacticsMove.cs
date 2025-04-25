using UnityEngine;
using System.Collections.Generic;

public class LTacticsMove : MonoBehaviour
{
    List<Tile> selectTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currTile;

    private UnitData unitData;
    private Animator unitAnimator;

    public bool moving=false;
    public int move;
    //public float jumpH=2;
    public int moveSpeed;

    Vector3 velo=new Vector3();
    Vector3 compass = new Vector3();

    float halfHeight=0;


    public void init(UnitData data, Animator animator)
    {
        //dev note: move line directly below into top of CompAdjLists() for 
        //manipulated tiles
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        unitData = data;
        unitAnimator = animator;

        move = unitData.moveDistance;
        moveSpeed = unitData.moveSpeed;


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

    public void CompAdjLists()
    {
        //place here if applicable
        foreach (GameObject ttile in tiles)
        {
            Tile t = ttile.GetComponent<Tile>();
            t.Neighbors();
        }
    }

    public void FindTilesBST()
    {
        CompAdjLists();
        GetCurrTile();

        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currTile);
        currTile.visited=true;
        //change parent maybe????? may lead to itself
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
        moving = true;
        unitAnimator.SetBool("isMoving", true);

        Tile next = tile;
        while(next!=null)
        {
            path.Push(next);
            next = next.par;
        }
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
            }
            else
            {
                //center reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelcTiles();
            moving = false;
            unitAnimator.SetBool("isMoving", false);
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

}
