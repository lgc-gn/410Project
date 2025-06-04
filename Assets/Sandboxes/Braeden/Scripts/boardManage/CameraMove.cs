using UnityEngine;
using System;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float zoomSpeed = 10f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 15f;
    public Transform cameraTarget; // Usually the player (this object)

    void Start()
    {
        // Auto-assign if not set
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (cameraTarget == null)
            cameraTarget = this.transform;

        
    }

    void Update()
    {
        HandleMovement();
        //HandleZoom();
    }

    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        // Get camera-relative movement vectors, ignoring Y
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camRight * inputX + camForward * inputZ).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        // Optional: rotate to face movement direction
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            // Direction from camera to target
            Vector3 direction = (cameraTarget.transform.position - transform.position).normalized;

            // Calculate new position
            Vector3 newPosition = cameraTarget.position + direction * scrollInput * zoomSpeed;

            // Clamp distance to min/max
            float distance = Vector3.Distance(newPosition, cameraTarget.transform.position);
            if (distance >= minZoomDistance && distance <= maxZoomDistance)
            {
                transform.position = newPosition;
            }
        }
    }
}

