using UnityEngine; 
using System.Collections.Generic;
[RequireComponent(typeof(UnitCursor))]

public class TurnOrderHandler : MonoBehaviour
{
    public Queue<Unit> turnOrderQueue = new Queue<Unit>();
    public List<GameObject> unitList = new List<GameObject>();  // Use List instead of array for dynamic additions

    public Unit turnUnit;
    public UnitCursor cursor;
    public TurnRecord record;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        CreateAQueue();
        record.Init(unitList.ConvertAll(u => u.GetComponent<Unit>()));

        //DEBUGReturnQueue(turnOrderQueue);

        StartTurn();
        //cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<UnitCursor>();
    }

    
    // Update is called once per frame
    void Update()
    {

        if (turnOrderQueue.Count == 0)
        {
            return;
        }

        Unit current = turnOrderQueue.Peek();
        cursor.SetActiveUnit(current);
        if (Input.GetKey(KeyCode.Space))
        {
            record.UndoMove(turnUnit);
        }


        if (current != null && current.unitData.activeTurn == false)
        {
            End_of_Turn();
        }
    }

    public void CreateAQueue()
    {
        GameObject[] unitObjectList = GameObject.FindGameObjectsWithTag("Unit");
        
        foreach (GameObject obj in unitObjectList)
        {
          
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null)
            {
                unitList.Add(obj);
            }
        }

        unitList.Sort((a, b) => b.GetComponent<Unit>().unitData.Initiative.CompareTo(a.GetComponent<Unit>().unitData.Initiative));

        // Sort the unit list by initative, and then add to the queue

        turnOrderQueue.Clear();

        foreach (GameObject obj in unitList)
        {
            turnOrderQueue.Enqueue(obj.GetComponent<Unit>());
        }
    }

    // Check if turn is done
    public bool IsTurnDone() => turnOrderQueue.Count <= 0;


    public Unit GetNextCharacter()
    {
        Unit activeTurnUnit = turnOrderQueue.Peek();

        if (IsTurnDone())
            return null;
        return turnOrderQueue.Dequeue();
    }

    public void RequeueUnit()
    {
        Unit requeuedUnit = turnOrderQueue.Peek();

        turnOrderQueue.Dequeue();

        turnOrderQueue.Enqueue(requeuedUnit);

        //print($"Requeued: {requeuedUnit.unitData.characterName}");
    }

    public void StartTurn()
    {

        Unit currentUnit = turnOrderQueue.Peek();
        //currentUnit.unitData.activeTurn = true;
        
        if (currentUnit != null)
        {
            //Debug.Log("Starting turn for: " + currentUnit.name);
            turnUnit = currentUnit;
            currentUnit.BeginTurn();
        }

        else
        {
            Debug.LogWarning("No unit available for turn!");
        }
    }


    public void End_of_Turn()
    {
        // If the turn queue is empty, just return.
        if (turnOrderQueue.Count == 0)
        {
            Debug.LogWarning("No units left in the turn queue!");
            return;
        }

        record.RecordPositions(unitList);

        //Unit finished = turnOrderQueue.Dequeue(); // Remove the unit from the front of the queue
        Debug.Log("Finished turn for: " + turnUnit.name);
    
        turnUnit.EndTurn();  // Mark the unit as finished

        // Re-enqueue the unit for the next turn cycle.
        // turnOrderQueue.Enqueue(finished);  // Add the finished unit back to the queue

        RequeueUnit();

        DEBUGReturnQueue(turnOrderQueue);
        
        if (turnOrderQueue.Count > 0)
        {
            Unit next = turnOrderQueue.Peek();  // Peek at the next unit
            Debug.Log("Starting turn for: " + next.name);
            next.BeginTurn();  // Begin the next unit's turn
        }
        else
        {
            Debug.LogWarning("The turn queue is empty!");
        }
    }



    #region debug

    public void DEBUGReturnQueue(Queue<Unit> queuetocheck)
    {
        string result = "Turn order: ";

        foreach (Unit unit in queuetocheck)
        {
            result += unit.unitData.characterName + ", ";
        }

        print(result);
    }

    #endregion

}


#region Unused Stuff

/*

    private static void Quick_Sort(GameObject[] arr, int left, int right)
    {
        // Check if there are elements to sort
        if (left < right)
        {
            int pivot = Partition(arr, left, right);

            // Recursively sort elements on the left and right of the pivot
            Quick_Sort(arr, left, pivot - 1);
            Quick_Sort(arr, pivot + 1, right);
        }
    }

    private static int Partition(GameObject[] arr, int left, int right)
    {
        // Select the pivot element (choose the first element in this case)
        int pivot = arr[left].GetComponent<LTacticsMove>().moveSpeed;

        while (true)
        {
            // Move left pointer until a value smaller than or equal to pivot is found
            while (arr[left].GetComponent<LTacticsMove>().moveSpeed > pivot)  // Changed to > for descending order
            {
                left++;
            }

            // Move right pointer until a value greater than or equal to pivot is found
            while (arr[right].GetComponent<LTacticsMove>().moveSpeed < pivot)  // Changed to < for descending order
            {
                right--;
            }

            // If left pointer is still smaller than right pointer, swap elements
            if (left < right)
            {
                GameObject temp = arr[left];
                arr[left] = arr[right];
                arr[right] = temp;
            }
            else
            {
                // Return the right pointer, indicating the partitioning position
                return right;
            }
        }
    }
*/

#endregion