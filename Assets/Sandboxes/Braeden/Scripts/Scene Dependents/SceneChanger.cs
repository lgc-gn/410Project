using UnityEngine;
using UnityEngine.SceneManagement; // Required for SceneManager

public class SceneChanger : MonoBehaviour
{
    // Method to load a scene by its name
    public void ChangeSceneByName(string sceneName)
    {
        // Check if the scene exists
        if (SceneManager.GetSceneByName(sceneName).IsValid())
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene not found: " + sceneName);
        }
    }

    // Method to load a scene by its build index
    public void ChangeSceneByIndex(int sceneIndex)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

    // Calculate the next scene index (wrap around if needed)
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        // Check if the index is valid
        if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("Scene index out of bounds: " + sceneIndex);
        }
    }

    // Example of how to use buttons to change scenes via UI
    public void OnButtonPressChangeScene(string sceneName)
    {
        ChangeSceneByName(sceneName);
    }
}
