using UnityEngine;
using Unity.Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    public CinemachineOrbitalFollow orbitalFollow; // Reference your Cinemachine Orbital Follow component here
    public float zoomSpeed = 5f;
    public float minRadius = 2f;
    public float maxRadius = 20f;

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            orbitalFollow.Radius -= scrollInput * zoomSpeed;
            orbitalFollow.Radius = Mathf.Clamp(orbitalFollow.Radius, minRadius, maxRadius);
        }
    }
}
