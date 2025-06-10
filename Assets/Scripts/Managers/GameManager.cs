using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    [Header("UI References")]
    [SerializeField] private PauseMenuController pauseMenuController;

    private bool isPaused = false;
    //private PauseMenuController pauseMenuController;//

    private void Start()
    {
        pauseMenuController = Object.FindFirstObjectByType<PauseMenuController>();

        // Auto-set to Playing if current scene is Game scene  
        if (SceneManager.GetActiveScene().name == "Game" && CurrentState != GameState.Playing)
        {
            Debug.Log("Auto setting state to Playing");
            CurrentState = GameState.Playing;
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Debug.Log("GameManager Update | State: " + CurrentState);

        if (CurrentState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("P key pressed");
                TogglePause();
            }
        }

        if (CurrentState == GameState.GameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed in GameOver state");
            LoadTitle();
        }
    }

    public bool IsPaused => isPaused;

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f; // Ensure time resumes
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameOver");
    }

    public void LoadTitle()
    {
        CurrentState = GameState.Title;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    public void ResetGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");

        if (pauseMenuController != null)
        {
            if (isPaused)
                pauseMenuController.ShowPauseMenu();
            else
                pauseMenuController.HidePauseMenu();
        }
        else
        {
            Debug.LogWarning("PauseMenuController not found. Cannot toggle pause menu visibility.");
        }
    }


    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
