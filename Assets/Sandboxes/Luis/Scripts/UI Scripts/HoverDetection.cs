using UnityEngine;

public class HoverDetection : MonoBehaviour
{
    private GameObject currentHoveredObject;
    public UIManager UI;
    private bool hoverState;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.GetComponent<Unit>() != null)
            {
                if (hitObject != currentHoveredObject)
                {
                    if (currentHoveredObject != null)
                    {
                        Debug.Log("Mouse is no longer on " + currentHoveredObject.name);
                        hoverState = false;

                    }
                    currentHoveredObject = hitObject;
                }

                // Log every frame while hovering the same unit
                Debug.Log("Mouse is over " + currentHoveredObject.name);
                hoverState = true;
                AdjStats(currentHoveredObject.GetComponent<Unit>());
            }
            else
            {
                if (currentHoveredObject != null)
                {
                    Debug.Log("Mouse is no longer on " + currentHoveredObject.name);
                    currentHoveredObject = null;
                    hoverState = false;
                }
            }
        }
        else
        {
            if (currentHoveredObject != null)
            {
                Debug.Log("Mouse is no longer on " + currentHoveredObject.name);
                currentHoveredObject = null;
                hoverState = false;
            }
        }
    }

    void AdjStats(Unit uni)
    {
        if (!hoverState)
        {
            UI.StatsBoard.SetActive(false);
        }
        else
        {
            UI.StatsBoard.SetActive(true);
            UI.ShowUnitInfo(uni);
        }
    }
}


