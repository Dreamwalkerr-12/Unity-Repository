using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button quitButton;  // <-- New Quit Game button

    private void Start()
    {
        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
        });

        quitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.QuitGame();
        });

        pauseMenuUI.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        Debug.Log("Pause menu shown");
    }

    public void HidePauseMenu()
    {
        pauseMenuUI.SetActive(false);
        Debug.Log("Pause menu hidden");
    }
}
