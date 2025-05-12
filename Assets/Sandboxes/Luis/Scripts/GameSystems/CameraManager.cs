using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance; // Ensure only one CameraManager can exist at a time.
    public CinemachineCamera unitCamera;
    private PlayerControls playerControls;

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
    }
}
