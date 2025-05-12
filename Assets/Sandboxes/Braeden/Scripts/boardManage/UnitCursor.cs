using UnityEngine;

public class UnitCursor : MonoBehaviour
{
    public float hoverHeight = 1f; // Height above the unit
    public float moveSpeed = 5f;   // Speed at which the cursor moves
    public Color highlightColor = Color.green; // Color to highlight the active unit

    private Renderer cursorRenderer;
    private Unit currentUnit;

    void Start()
    {
        cursorRenderer = GetComponent<Renderer>();
        if (cursorRenderer != null)
        {
            cursorRenderer.enabled = false; // Hide cursor initially
        }
    }

    void Update()
    {
        if (currentUnit != null)
        {
            // Calculate the target position above the unit
            Vector3 targetPosition = currentUnit.transform.position + Vector3.up * hoverHeight;

            // Move the cursor smoothly to the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Optionally, change the cursor's color to indicate the active unit
            if (cursorRenderer != null)
            {
                cursorRenderer.enabled = true;
                cursorRenderer.material.color = highlightColor;
            }
        }
    }

    public void SetActiveUnit(Unit unit)
    {
        currentUnit = unit;
    }
}
