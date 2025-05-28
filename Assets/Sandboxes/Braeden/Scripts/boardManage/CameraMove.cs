using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CameraMove : MonoBehaviour
{

    public float moveSpeed = 5f; // Speed of movement
    public float rotSpeed = 100f;
    public float zoomSpeed = 10f;
    public GameObject target;
    public float max = 100;
    public float min = 1;
    void Update()
    {
        // Get input from WASD or arrow keys
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Create movement vector
        Vector3 LR = transform.right * moveX;
        Vector3 UD = transform.forward * moveZ;
        Vector3 move = LR + UD;



        // Apply movement
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            // Direction from camera to target
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Calculate new position
            Vector3 newPosition = transform.position + direction * scrollInput * zoomSpeed;

            // Clamp distance to min/max
            float distance = Vector3.Distance(newPosition, target.transform.position);
            if (distance >= min && distance <= max)
            {
                transform.position = newPosition;
            }
        }
    }
}

