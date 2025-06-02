using UnityEngine;
using System.Collections.Generic;

public class TileUpkeep : MonoBehaviour
{
    public List<Tile> tileList = new List<Tile>();

    public void Start()
    {
        // Optionally auto-populate tile list
        if (tileList.Count == 0)
        {
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
            foreach (GameObject obj in tiles)
            {
                Tile t = obj.GetComponent<Tile>();
                if (t != null)
                    tileList.Add(t);
            }
        }

        foreach (Tile tild in tileList)
        {
            RaycastHit hit;
            if (Physics.Raycast(tild.transform.position, Vector3.up, out hit, 1))
            {
                Unit uni = hit.collider.GetComponent<Unit>();
                tild.occupied = uni;
            }
        }
    }

    /// <summary>
    /// Call this once at the end of every turn to refresh which unit is standing on which tile.
    /// </summary>
    public void UpdateTileOccupancy()
    {
        foreach (Tile t in tileList)
        {
            //Debug.Log(t.name);
            t.occupied = null; // Reset

            RaycastHit hit;
            if (Physics.Raycast(t.transform.position, Vector3.up, out hit, .5f))
            {
                Unit uno = hit.collider.GetComponent<Unit>();
                if (uno != null)
                {
                    t.occupied = uno;
                }
            }
        }
    }
}
