using UnityEngine;

/*

UNIT Method Script

Pulls data from inputted UnitData scriptable object



*/

public class Unit : LTacticsMove
{

    public UnitData data; // Pass in the UnitData scriptableobject
    public Animator animator;

    public string characterName, characterClass;


    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        animator = GetComponent<Animator>();

        init(data, animator);
        currentHealth = data.maxHealth;
        characterName = data.characterName;
        characterClass = data.characterClass;

        print($"Loaded character: {characterName}, {characterClass}");
    }

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
