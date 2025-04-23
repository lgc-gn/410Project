using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tile : MonoBehaviour
{
    //movement flags
    public bool current=false;
    public bool target=false;
    public bool selectable=false;

    //general flags
    public bool walk = true;

    //BFS strategy
    public List<Tile> adjList = new List<Tile>();

    public bool visited = false;
    public Tile par = null;
    public int dist=0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame, manually loaded or fixed on tile type
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color=Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color=Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color=Color.white;
        }
        
    }
    //clears position for new movement when selected
    public void Reset()
    {
    adjList.Clear();
    current=false;
    target=false;
    selectable=false;

    //general flags
    walk = true;

    //BFS strategy
    visited = false;
    par = null;
    dist=0;
    }
    //checks neighboring tiles existing
    public void Neighbors(/*float jump*/)
    {
        Reset();
        CheckTile(Vector3.forward);
        CheckTile(-Vector3.forward);
        CheckTile(Vector3.right);
        CheckTile(-Vector3.right);
    }
    //checks tile type, tag, and contents
    public void CheckTile(Vector3 direction /*jumpheight*/)
    {
        Vector3 halfEx = new Vector3(0.25f, 0.25f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position+direction, halfEx);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile!=null && tile.walk)
            {
                RaycastHit hit;
                if(!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    adjList.Add(tile);
                }
            }
        }
    }

}
