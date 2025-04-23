using UnityEngine;

public class player : TacticsMove
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!moving)
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
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    if(t.selectable)
                    {
                        //move code
                        MoveToTile(t);
                    }
                }
            }
        }
    }
}
