using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button aboutButton;
    public Button closeButton;

    public GameObject main;
    public GameObject about;

    void onAboutButtonPress()
    {
        main.SetActive(false);
        about.SetActive(true);
    }

    void onPlayButtonPress()
    {
        print("Play button");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void onCloseButtonPress()
    {
        main.SetActive(true);
        about.SetActive(false);
    }

    private void Start()
    {
        playButton.onClick.AddListener(onPlayButtonPress);
        aboutButton.onClick.AddListener(onAboutButtonPress);
        closeButton.onClick.AddListener(onCloseButtonPress);

        print(SceneManager.GetActiveScene().name);
    }
}
