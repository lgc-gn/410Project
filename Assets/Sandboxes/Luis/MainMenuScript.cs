using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{

    public GameObject AboutPanel, MainPanel;
    public Button CloseButton, PlayButton, AboutButton;

    void CloseAbout()
    {
        AboutPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void OpenAbout()
    {
        AboutPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    void Start()
    {
        CloseButton.onClick.AddListener(CloseAbout);
        PlayButton.onClick.AddListener(PlayGame);
        AboutButton.onClick.AddListener(OpenAbout);
    }


}
