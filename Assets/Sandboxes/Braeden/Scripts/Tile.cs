using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile : MonoBehaviour
{
    //movement flags
    public bool current=false;
    public bool target=false;
    public bool selectable=false;
    public bool AOE = false;

    //general flags

    //BFS strategy
    public List<Tile> adjList = new List<Tile>();

    public bool visited = false;
    public Tile par = null;
    public int dist=0;

    //for enemy AI
    public float f =0;
    public float g = 0;
    public float h =0;

    public Unit occupied=null;

    public bool attackstate=false;

    public TileData tileData;

    Color newTileColor;

    private Color baseColor;

    public bool walk
    {
        get
        {
            if (tileData != null)
                return tileData.walk;
            else
                return true; // default if no tileData assigned
        }
        set
        {
            if (tileData != null)
                tileData.walk = value;
            // else ignore or handle error
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ColorUtility.TryParseHtmlString("#43A7CC", out newTileColor)) { }

        if (tileData != null)
        {
            baseColor = tileData.tileColor;
            walk = tileData.walk;
        }
        else
        {
            baseColor = GetComponent<Renderer>().material.color;
        }

        GetComponent<Renderer>().material.color = baseColor;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1))
        {
            occupied = hit.collider.GetComponent<Unit>();
        }
    }


    // Update is called once per frame, manually loaded or fixed on tile type
    void Update()
    {
        Color highlightColor = baseColor;

        // current and target colors (unchanged)
        if (current)
        {
            highlightColor = Color.Lerp(highlightColor, Color.black, 0.5f);
        }
        if (target)
        {
            highlightColor = Color.Lerp(highlightColor, Color.green, 0.5f);
        }

        // For selectable tiles:
        if (selectable)
        {
            if (attackstate)
            {
                // selectable + attackstate = red color
                highlightColor = Color.Lerp(highlightColor, Color.red, 0.7f);
            }
            else
            {
                // selectable but not attackstate, use your usual selectable color
                highlightColor = Color.Lerp(highlightColor, newTileColor, 0.5f);
            }
        }

        // AOE color blend remains if you want it:
        if (AOE)
        {
            highlightColor = Color.Lerp(highlightColor, Color.red, 0.5f);
        }

        GetComponent<Renderer>().material.color = highlightColor;
    }





    //clears position for new movement when selected
    public void Reset()
    {
        adjList.Clear();
        current = false;
        target = false;
        selectable = false;
        g = h = f = 0;
        par = null;

        //general flags
        //walk = true;

        //BFS strategy
        visited = false;
        par = null;
        dist = 0;

        f = g = h = 0;

        AOEreset();
    }

    public void AOEreset()
    {
        AOE = false;
    }
    //checks neighboring tiles existing
    public void Neighbors(Tile target)
    {
        Reset();
        CheckTile(Vector3.forward, target);
        CheckTile(-Vector3.forward, target);
        CheckTile(Vector3.right, target);
        CheckTile(-Vector3.right, target);
    }
    //checks tile type, tag, and contents
    public void CheckTile(Vector3 direction, Tile target)
    {
        Vector3 halfEx = new Vector3(0.25f, 0.25f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfEx);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null)
            {
                if (attackstate)
                {
                    // In attack mode, include all tiles — even if occupied or not walkable
                    adjList.Add(tile);
                }
                else if (tile.walk)
                {
                    // In move mode, only include walkable and unoccupied tiles (or the target tile)
                    RaycastHit hit;
                    if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                    {
                        adjList.Add(tile);
                    }
                }
            }
        }
    }


}
