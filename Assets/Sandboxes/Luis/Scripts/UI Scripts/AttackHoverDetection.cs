using UnityEngine;
using System.Collections.Generic;

public class AttackHoverDetection : MonoBehaviour
{
    private GameObject currentHoveredObject;
    public UIManager UI;
    public Unit currUnit;
    Tile currTile;
    GameObject[] tiles;

//this bool controls on/off state
    public bool readyAOE;

    List<Tile> prevAOETiles = new List<Tile>();


    void Start()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        currUnit = UI.TurnOrderScript.ReturnCurrentQueue().Peek();

    }

    void Update()
    {
        GameObject primary = UI.primaryCam;

        if (!primary.TryGetComponent<Camera>(out Camera camera))
        {
            primary = primary.transform.GetChild(0).gameObject;
            primary.TryGetComponent<Camera>(out camera);
        }

        if (camera == null)
            return;
        if (readyAOE)
        {
            currUnit.attack_state = true;
            Ray ray = camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.GetComponent<Tile>() != null)
                {
                    if (hitObject != currentHoveredObject)
                    {
                        if (currentHoveredObject != null)
                        {
                            ClearAOEFlags();
                        }
                        currentHoveredObject = hitObject;
                    }

                    currTile = currentHoveredObject.GetComponent<Tile>();

                    ClearAOEFlags();

                    // Set attackstate for all tiles before BFS
                    SetAllTilesAttackState(currUnit.attack_state);

                    FindAOETilesBST(currTile, 2);
                }
                else
                {
                    if (currentHoveredObject != null)
                    {
                        ClearAOEFlags();
                        currentHoveredObject = null;
                    }
                }
            }
            else
            {
                if (currentHoveredObject != null)
                {
                    ClearAOEFlags();
                    currentHoveredObject = null;
                    ClearSelectableFlags();
                }
            }
        }
        else
        {
            Debug.Log("lol");
            ClearAOEFlags();
        }
    }

    void ClearAOEFlags()
    {
        foreach (Tile tile in prevAOETiles)
        {
            tile.AOE = false; // Do NOT touch tile.selectable here
        }
        prevAOETiles.Clear();
    }



    void SetAllTilesAttackState(bool attackState)
    {
        foreach (GameObject tileObj in tiles)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            tile.attackstate = attackState;
        }
    }

    public void FindAOETilesBST(Tile currTile, int range)
    {
        if (currTile == null)
            return;

        // Reset BFS helpers
        foreach (GameObject tileObj in tiles)
        {
            Tile t = tileObj.GetComponent<Tile>();
            t.visited = false;
            t.dist = 0;
            // Do NOT clear AOE here to avoid removing prior flags prematurely
        }

        AOECompAdjLists(currTile);

        Queue<Tile> queue = new Queue<Tile>();
        currTile.visited = true;
        currTile.dist = 0;
        currTile.AOE = true;
        queue.Enqueue(currTile);

        while (queue.Count > 0)
        {
            Tile t = queue.Dequeue();

            if (t.dist < range)
            {
                foreach (Tile neighbor in t.adjList)
                {
                    if (!neighbor.visited)
                    {
                        neighbor.visited = true;
                        neighbor.dist = t.dist + 1;
                        neighbor.AOE = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        // Record tiles that now have AOE true
        prevAOETiles.Clear();
        foreach (GameObject tileObj in tiles)
        {
            Tile t = tileObj.GetComponent<Tile>();
            if (t.AOE)
                prevAOETiles.Add(t);
        }
    }


    public void AOECompAdjLists(Tile target)
    {
        foreach (GameObject tileObj in tiles)
        {
            Tile t = tileObj.GetComponent<Tile>();
            t.Neighbors(target); // update neighbors if needed
        }
    }

    public void ClearSelectableFlags()
    {
        foreach (GameObject tileObj in tiles)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            tile.selectable = false;
        }
    }

}
