using UnityEngine;

public class HoverDetector : MonoBehaviour
{
    Unit lastHoveredUnit;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Filter ONLY objects tagged as "Unit"
            if (hit.collider.CompareTag("Unit"))
            {
                Unit unit = hit.collider.GetComponent<Unit>();
                if (unit != null && unit != lastHoveredUnit)
                {
                    lastHoveredUnit = unit;
                    UIManager.Instance.ShowUnitInfo(unit.data);
                }
            }
            else if (lastHoveredUnit != null)
            {
                lastHoveredUnit = null;
                UIManager.Instance.HideUnitInfo();
            }
        }
        else if (lastHoveredUnit != null)
        {
            lastHoveredUnit = null;
            UIManager.Instance.HideUnitInfo();
        }
    }
}
