using UnityEngine; 
using System.Collections.Generic;

public class TurnRecord : MonoBehaviour
{
    public Dictionary<string, List<Vector3>> undo = new Dictionary<string, List<Vector3>>();

    public void Init(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            undo[unit.unitData.characterName] = new List<Vector3> { unit.transform.position };
        }
    }

    // This will store the positions of all units after each turn
    public void RecordPositions(List<GameObject> units)
    {
        foreach (GameObject uni in units)
        {
            Unit unit = uni.GetComponent<Unit>();
            string key = unit.unitData.characterName;

            if (!undo.ContainsKey(key))
            {
                undo[key] = new List<Vector3>();  // Initialize if it doesn't exist
            }

            undo[key].Add(unit.transform.position);  // Record position
            Debug.Log($"Recorded position for {key}: {unit.transform.position}");
        }
    }

    // Undo the last position for a given unit
    public void UndoMove(Unit unit)
    {
        if (!undo.ContainsKey(unit.unitData.characterName))
        {
            Debug.LogWarning("No previous positions to undo to.");
            return;
        }

        List<Vector3> history = undo[unit.unitData.characterName];

        if (history.Count > 1)
        {
            history.RemoveAt(history.Count - 1);  // Remove the current position
            Vector3 lastPosition = history[history.Count - 1];  // Get the previous position
            unit.transform.position = lastPosition;
            Debug.Log($"Moved {unit.unitData.characterName} to {lastPosition}");
        }
        else
        {
            Debug.Log("No previous positions to undo to.");
        }
    }
}
