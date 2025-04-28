using UnityEngine;

/*

UNIT Method Script

Handles player control of units

*/

public class Unit : TacticalUnitBase
{

    private void Awake()
    {

        movementController = GetComponent<LTacticsMove>();
        animator = GetComponent<Animator>();

        movementController.init(this, animator);
    }

    public void BeginTurn()
    {

        unitData.activeTurn = true;

    }

    public void EndTurn()
    {

        unitData.activeTurn = false;
        unitData.isMoving = false;
        animator.SetBool("isMoving", false);

        // Add additional logic if needed to reset other state variables
    }
    
    public virtual void HandleMoveCommand()
    {
        if (!unitData.activeTurn)
        {
            return;
        }

        if (!unitData.isMoving)
        {
            movementController.FindTilesBST();
            CheckMouse();
        }

        else
        {
            movementController.Move();
        }
    }

    void Update()
    {

        HandleMoveCommand();
    }

    #region Helper Functions


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

                        movementController.MoveToTile(t);
                    }
                }
            }
        }
    }

    #endregion

}
