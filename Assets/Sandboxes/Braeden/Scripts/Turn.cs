using UnityEngine;  // Make sure to include this namespace

using System.Collections.Generic;
using System.Collections;

public class Turn : Unit
{
    Queue<LTacticsMove> turnQueue = new Queue<LTacticsMove>();
    List<GameObject> unitList = new List<GameObject>();  // Use List instead of array for dynamic additions
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateAQueue();
        foreach (LTacticsMove unit in turnQueue)
        {
            Debug.Log(unit.moveSpeed);
        }
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (turnQueue.Count == 0)
        {
            return;
        }

        LTacticsMove current = turnQueue.Peek();
        if (current != null && !current.turn)
        {
            End_of_Turn();
        }
    }

    public void CreateAQueue()
    {
        GameObject[] playList = GameObject.FindGameObjectsWithTag("Unit");
        GameObject[] nmeList = GameObject.FindGameObjectsWithTag("NME");
        
        foreach (GameObject obj in playList)
        {
            LTacticsMove unit = obj.GetComponent<LTacticsMove>();
            if (unit != null)
            {
                unitList.Add(obj);  // Add GameObject to the list
                turnQueue.Enqueue(unit);  // Add LTacticsMove component to the queue
            }
        }

        foreach (GameObject obj in nmeList)
        {
            LTacticsMove unit = obj.GetComponent<LTacticsMove>();
            if (unit != null)
            {
                unitList.Add(obj);  // Add GameObject to the list
                turnQueue.Enqueue(unit);  // Add LTacticsMove component to the queue
            }
        }

        // Sort the unitList by moveSpeed of LTacticsMove components
        unitList.Sort((a, b) => b.GetComponent<LTacticsMove>().moveSpeed.CompareTo(a.GetComponent<LTacticsMove>().moveSpeed));
    }

    // Check if turn is done
    public bool IsTurnDone() => turnQueue.Count <= 0;

    // Get the next character to move
        // Get the next character to move
    public LTacticsMove GetNextCharacter()
    {
        if (IsTurnDone())
            return null;
        return turnQueue.Dequeue();  // Dequeue LTacticsMove, not GameObject
    }

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

    void StartTurn()
    {
        LTacticsMove current = turnQueue.Peek();
        if (current != null)
        {
            Debug.Log("Starting turn for: " + current.name);
            current.BeginTurn();
        }
        else
        {
            Debug.LogWarning("No unit available for turn!");
        }
    }


    public void End_of_Turn()
    {
        // If the turn queue is empty, just return.
        if (turnQueue.Count == 0)
        {
            Debug.LogWarning("No units left in the turn queue!");
            return;
        }

        LTacticsMove finished = turnQueue.Dequeue(); // Remove the unit from the front of the queue
        Debug.Log("Finished turn for: " + finished.name);
    
        finished.EndTurn();  // Mark the unit as finished

        // Re-enqueue the unit for the next turn cycle.
        turnQueue.Enqueue(finished);  // Add the finished unit back to the queue

        // Check if there are still units in the queue to start the next turn
        if (turnQueue.Count > 0)
        {
            LTacticsMove next = turnQueue.Peek();  // Peek at the next unit
            Debug.Log("Starting turn for: " + next.name);
            next.BeginTurn();  // Begin the next unit's turn
        }
        else
        {
            Debug.LogWarning("The turn queue is empty!");
        }
    }


}

