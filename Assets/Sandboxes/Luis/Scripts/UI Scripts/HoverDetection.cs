using UnityEngine;

public class HoverDetection : MonoBehaviour
{
    private GameObject currentHoveredObject;
    public UIManager UI;
    //on/off state
    public bool hoverState;
    public GameObject primary;
    public Unit defaultUni;

    void Update()
    {
        primary = UI.primaryCam;
        if (!primary.TryGetComponent<Camera>(out Camera camera))
        {
            if (primary.transform.childCount > 0)
            {
                primary = primary.transform.GetChild(0).gameObject;
            }
            else
            {
                Debug.LogWarning($"{primary.name} has no children!");
            }

        }

        if (hoverState)
        {
            Ray ray = primary.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
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

                        }
                        currentHoveredObject = hitObject;
                    }

                    // Log every frame while hovering the same unit
                    //Debug.Log("Mouse is over " + currentHoveredObject.name);
                    AdjStats(currentHoveredObject.GetComponent<Unit>());
                }
                else
                {
                    if (currentHoveredObject != null)
                    {
                        //Debug.Log("Mouse is no longer on " + currentHoveredObject.name);
                        currentHoveredObject = null;
                    }
                }
            }
            else
            {
                if (currentHoveredObject != null)
                {
                    //Debug.Log("Mouse is no longer on " + currentHoveredObject.name);
                    currentHoveredObject = null;
                }
            }
        }
        //else if (!hoverState)
        //{
        //    AdjStats(defaultUni);
        //}
    }

    void AdjStats(Unit uni)
    {
        UI.ShowUnitInfo(uni);
    }
}


