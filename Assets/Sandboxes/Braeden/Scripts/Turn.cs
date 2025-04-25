using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Turn : Unit
{
    Queue<Unit> turnQueue = new Queue<Unit>();  // Change to Queue<Unit> to store Unit components
    List<GameObject> unitList = new List<GameObject>();  // Use List instead of array for dynamic additions
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateAQueue();
        foreach (Unit unit in turnQueue)
        {
            Debug.Log(unit.moveSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Need to constantly run a process here to check for updated speed values to 
        // keep turn order consistent
    }

    public void CreateAQueue()
    {
        GameObject[] playList = GameObject.FindGameObjectsWithTag("Unit");
        GameObject[] nmeList = GameObject.FindGameObjectsWithTag("NME");
        
        foreach (GameObject obj in playList)
        {
            unitList.Add(obj);  // Use Add method for List
        }
        
        foreach (GameObject obj in nmeList)
        {
            unitList.Add(obj);  // Corrected from nmeListyList to nmeList
        }
        
        // Sort the unitList using Quick_Sort (convert List to array for sorting)
        GameObject[] unitArray = unitList.ToArray();  // Convert List to array for sorting
        Quick_Sort(unitArray, 0, unitArray.Length - 1);  // Sort the array by moveSpeed
    
        // Create a new queue and populate it with the sorted units
        Queue<Unit> sortedQueue = new Queue<Unit>();
    
        // Add the sorted units (Unit components) to the queue
        foreach (GameObject obj in unitArray)
        {
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null)
            {
                sortedQueue.Enqueue(unit);
            }
        }

        // Assign the sorted queue back to turnQueue (which is now Queue<Unit>)
        turnQueue = sortedQueue;
    }

    // Check if turn is done
    public bool IsTurnDone() => turnQueue.Count <= 0;

    // Get the next character to move
    public Unit GetNextCharacter()  // Change return type to Unit
    {
        if (IsTurnDone())
            return null;
        return turnQueue.Dequeue();  // Dequeue Unit, not GameObject
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
        int pivot = arr[left].GetComponent<Unit>().moveSpeed;

        while (true)
        {
            // Move left pointer until a value smaller than or equal to pivot is found
            while (arr[left].GetComponent<Unit>().moveSpeed > pivot)  // Changed to > for descending order
            {
                left++;
            }

            // Move right pointer until a value greater than or equal to pivot is found
            while (arr[right].GetComponent<Unit>().moveSpeed < pivot)  // Changed to < for descending order
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

}
