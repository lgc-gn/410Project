using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // Ensure only one CameraManager can exist at a time.
    public CinemachineCamera unitCamera;
    private PlayerControls playerControls;
    public float moveSpeed = 5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        playerControls = new PlayerControls();


    }


    public void UpdateCameraTracking(Unit newUnit)
    {
        unitCamera.Follow = newUnit.transform;
        // Get input from WASD or arrow keys
        /*float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Create movement vector
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // Apply movement
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);*/
    }
}
