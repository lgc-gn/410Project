using UnityEngine; 
using System.Collections.Generic;
using Unity.Cinemachine;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;
using TMPro;
>>>>>>> Stashed changes
[RequireComponent(typeof(UnitCursor))]

public class TurnOrderHandler : MonoBehaviour
{
    public Queue<Unit> turnOrderQueue = new Queue<Unit>();
    public List<GameObject> unitList = new List<GameObject>();  // Use List instead of array for dynamic additions
    public List<Tile> tileList = new List<Tile>();

    public Unit turnUnit;
    //public UnitCursor cursor;
    public TurnRecord record;
    public TileUpkeep upkeep;

    public UIManager UIManagerScript;
    public CameraManager CameraManagerScript;
    public CameraMove freecam;
    public GameObject winScreen;
    public SceneChanger scene;

    private bool winTriggered=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winScreen.SetActive(false);

        CreateAQueue();
        record.Init(unitList.ConvertAll(u => u.GetComponent<Unit>()));
        upkeep.Start();

        GameObject[] tileObjectList = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject til in tileObjectList)
        {
            tileList.Add(til.GetComponent<Tile>());
        }

        UIManagerScript.UpdateTurnOrderList(turnOrderQueue);
        UIManagerScript.ShowUnitInfo(turnOrderQueue.Peek());
        CameraManagerScript.UpdateCameraTracking(turnOrderQueue.Peek());

        //if (turnOrderQueue.Peek().unitData.Allied == true)
        //{
            CameraManagerScript.UpdateCameraTracking(turnOrderQueue.Peek());
            //StartCoroutine(UIManagerScript.SmoothMoveActionUI("left", .15f));
        //}

        ReturnQueue(turnOrderQueue);

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
        //cursor.SetActiveUnit(current);

        if (Input.GetKey(KeyCode.Y))
        {
            record.UndoMove(turnUnit);
        }

        if (current != null && current.unitData.activeTurn == false)
        {
            End_of_Turn();
        }

<<<<<<< Updated upstream
        if (!winTriggered)
        {
            CheckWinConditions();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            UIManagerScript.OnResetClicked();
        }


        //foreach (GameObject obj in unitList)
        //{
        //    Unit uni = obj.GetComponent<Unit>();
        //    if (uni.dead)
        //    {
        //        unitList.Clear();
        //        CreateAQueue();
        //    }
=======
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    UIManagerScript.OnResetClicked();
>>>>>>> Stashed changes
        //}
    }


    //ToDo: make a real lose con and add fail anim
//ToDo: update objectives UI for what is needed
    private void CheckWinConditions()
    {
        bool playerLordDead = false;
        bool anyEnemiesAlive = false;

        foreach (GameObject obj in unitList)
        {
            Unit uni = obj.GetComponent<Unit>();
            if (uni != null)
            {
                if (!uni.unitData.Dead)
                {
                    if (uni.NMEtag)
                    {
                        anyEnemiesAlive = true;
                    }
                    if (uni.unitData.Lord && !uni.NMEtag)
                    {
                        playerLordDead = false;
                    }
                }
                else if (uni.unitData.Lord && !uni.NMEtag)
                {
                    playerLordDead = true;
                }
            }
        }

        if (playerLordDead)
        {
            Debug.Log("Player Lord is dead. You lose (not implemented).");
            // TODO: Implement lose condition
            return;
        }

        if (!anyEnemiesAlive)
        {
<<<<<<< Updated upstream
            Debug.Log("All enemies are dead. You win!");
            winTriggered = true;
            winScreen.SetActive(true);
=======
            UIManagerScript.VictoryOverlay.SetActive(true);
>>>>>>> Stashed changes
            Invoke("MoveToNext", 5f);
        }
    }



    public void CreateAQueue()
    {
        GameObject[] unitObjectList = GameObject.FindGameObjectsWithTag("Unit");

        unitList.Clear(); // Clear previous list completely

        foreach (GameObject obj in unitObjectList)
        {
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null && !unit.unitData.Dead)
            {
                unitList.Add(obj);
            }
        }

        unitList.Sort((a, b) => b.GetComponent<Unit>().unitData.Initiative.CompareTo(a.GetComponent<Unit>().unitData.Initiative));

        turnOrderQueue.Clear();

        foreach (GameObject obj in unitList)
        {
            turnOrderQueue.Enqueue(obj.GetComponent<Unit>());
        }
    }


    void TileClear()
    {
        foreach (Tile til in tileList)
        {
            til.Reset();
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

        if (currentUnit != null )
        {
            turnUnit = currentUnit;
            freecam.transform.position = currentUnit.transform.position;
            currentUnit.BeginTurn();
        }
    }


    public void End_of_Turn()
    {
        if (turnOrderQueue.Count == 0)
        {
            Debug.LogWarning("No units left in the turn queue!");
            return;
        }

        record.RecordPositions(unitList);
        turnUnit.EndTurn();
        TileClear();
        upkeep.UpdateTileOccupancy();

        // Check if the unit is dead, and handle removal
        if (turnUnit.unitData.Dead)
        {
            Debug.Log($"{turnUnit.unitData.characterName} is dead and will be removed from the queue.");
            turnOrderQueue.Dequeue();
        }
        else
        {
            RequeueUnit();
        }

        UIManagerScript.UpdateTurnOrderList(turnOrderQueue);

        if (turnOrderQueue.Count > 0)
        {
            Unit next = turnOrderQueue.Peek();
            turnUnit = next;
            if (UIManagerScript.CamState)
            {
                UIManagerScript.ShowUnitInfo(next);
            }
            CameraManagerScript.UpdateCameraTracking(next);
            next.BeginTurn();
        }
        else
        {
            Debug.LogWarning("The turn queue is empty!");
        }
    }



    public string ReturnQueue(Queue<Unit> queuetocheck)
    {
        string result = "";

        foreach (Unit unit in queuetocheck)
        {
            result += unit.unitData.characterName + "\n";
        }

        return result;
    }

    public Queue<Unit> ReturnCurrentQueue()
    {
        return turnOrderQueue;
    }

    void MoveToNext()
    {
<<<<<<< Updated upstream
        scene.ChangeSceneByIndex(2);
=======

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            SceneManager.LoadScene(nextSceneIndex);
        }

>>>>>>> Stashed changes
    }

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