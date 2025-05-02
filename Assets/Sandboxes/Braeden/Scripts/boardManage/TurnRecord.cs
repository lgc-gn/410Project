using UnityEngine;
using System.Collections.Generic;

public class TurnRecord : MonoBehaviour
{
    // Use Unit as the key to ensure uniqueness
    public Dictionary<Unit, List<Vector3>> undo = new Dictionary<Unit, List<Vector3>>();

    // Initialize the position history for each unit
    public void Init(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            undo[unit] = new List<Vector3> { unit.transform.position };
        }
    }

    // Store the positions of all units after each turn
    public void RecordPositions(List<GameObject> units)
    {
        foreach (GameObject uni in units)
        {
            Unit unit = uni.GetComponent<Unit>();

            if (!undo.ContainsKey(unit))
            {
                undo[unit] = new List<Vector3>();
            }

            List<Vector3> history = undo[unit];
            history.Add(unit.transform.position);

// Limit history size to the last 3 positions
            int maxHistory = 3;
            if (history.Count > maxHistory)
            {
                history.RemoveAt(0); // Remove the oldest entry
            }

            Debug.Log($"Recorded position for {unit.unitData.characterName}: {unit.transform.position}");
        }
    }

    // Undo the last position for a given unit
    public void UndoMove(Unit unit)
    {
        if (!undo.ContainsKey(unit))
        {
            Debug.LogWarning("No previous positions to undo to.");
            return;
        }

        List<Vector3> history = undo[unit];

        if (history.Count > 1)
        {
            history.RemoveAt(history.Count - 1); // Remove the current position
            Vector3 lastPosition = history[history.Count - 1]; // Get the previous position
            unit.transform.position = lastPosition;
            Debug.Log($"Moved {unit.unitData.characterName} to {lastPosition}");
        }
        else
        {
            Debug.Log("No previous positions to undo to.");
        }
    }
}
