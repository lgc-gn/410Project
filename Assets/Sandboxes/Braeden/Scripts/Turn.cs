using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Turn : Unit
{
    //static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    //may not use above definition???
    Queue<Unit> turnQueue = new Queue<Unit>();
    private GameObject[] unitList;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateAQueue();
    }

    // Update is called once per frame
    void Update()
    {
        //need to constantly run a process here to check for updated speed values to 
        //keep turn order consistent
    }

    public void CreateAQueue()
    {
        //Get all turn takers
        //GameObject[] turnTakers = FindObjectsByType(typeof(Unit), FindObjectsSortMode.None);

        GameObject[] playList = GameObject.FindGameObjectsWithTag("Unit");
        Debug.Log("Hi");
        Debug.Log(playList);
        foreach (GameObject obj in playList)
        {
            Debug.Log("->" + obj.GetComponent<Unit>().characterName);
        }
        /*foreach(GameObject obj in playList)
        {
            unitList.append(obj)
        }*/

        //Sort it by speed from highest speed to lowest speed
        //ToDO: add .GetSpeed Func in units
        //turnTakers = turnTakers.OrderByDescending((val) => val.GetSpeed()).ToArray();

        //Additional sorting to ensure that player and enemy moves interchangably ?

        //Create a queue that will allow you to get the correct character first
        //turnQueue = new Queue<Unit>(turnTakers);
    }

    //Check if turn is done
    public bool IsTurnDone() => turnQueue.Count <= 0;

    //get the next character to move
    public Unit GetNextCharacter()
    {
        if (IsTurnDone())
            return null;
        return turnQueue.Dequeue();
    }

    //ill figure it out later
    //edit to account for speed times
    /*public int[] SortArray(int[] array, int leftIndex, int rightIndex)
    {
    var i = leftIndex;
    var j = rightIndex;
    var pivot = array[leftIndex];
    while (i <= j)
    {
        while (array[i] < pivot)
        {
            i++;
        }
        
        while (array[j] > pivot)
        {
            j--;
        }
            if (i <= j)
            {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
            i++;
            j--;
            }
        }

    }*/
}
