using UnityEngine;

public class HoverDetector : MonoBehaviour
{
    //[SerializeField] private Unit lastHoveredUnit;

    //private void Start()
    //{
    //    private Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    private RaycastHit hit;
    //}

    //void Update()
    //{

    //}

//    void HoverDetect()
//    {


//        if (Physics.Raycast(ray, out hit))
//        {

//            if (hit.collider.CompareTag("Unit"))
//            {
//                Unit unit = hit.collider.GetComponent<Unit>();
//                if (unit != null)
//                {

//                    if (unit != lastHoveredUnit)
//                    {
//                        lastHoveredUnit = unit;
//                        lastHoveredUnit.animator.SetBool("isSelected", true);

//                        UIManager.Instance.ShowUnitInfo(unit);
//                    }


//                    if (Input.GetMouseButtonDown(0))
//                    {
//                        OnUnitClicked(unit);
//                    }
//                }
//            }
//            else
//            {
//                ClearHover();
//            }
//        }
//        else
//        {
//            ClearHover();
//        }
//    }

//    void OnUnitClicked(Unit unit)
//    {
//        //Debug.Log($"Unit clicked: {unit.unitData.characterName}");

//    }

//    void ClearHover()
//    {
//        if (lastHoveredUnit != null)
//        {
//            lastHoveredUnit.animator.SetBool("isSelected", false);

//            lastHoveredUnit = null;
//            UIManager.Instance.HideUnitInfo();
//        }
//    }
}
