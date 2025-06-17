using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

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

    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool isPaused = false;

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

    private void Start()
    {
        if (pauseMenuController == null)
            pauseMenuController = Object.FindFirstObjectByType<PauseMenuController>();

        if (SceneManager.GetActiveScene().name == "Game" && CurrentState != GameState.Playing)
        {
            Debug.Log("Auto setting state to Playing");
            CurrentState = GameState.Playing;
        }
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }

        if (CurrentState == GameState.GameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            LoadTitle();
        }
    }

    public bool IsPaused => isPaused;

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        StartCoroutine(FadeOutMusicAndStart());
    }

    private IEnumerator FadeOutMusicAndStart()
    {
        float currentTime = 0f;
        masterMixer.GetFloat("MusicVolume", out float currentDb);
        float startVolume = Mathf.Pow(10f, currentDb / 20f);

        while (currentTime < fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            float newVolume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
            float volumeInDb = Mathf.Log10(Mathf.Max(newVolume, 0.0001f)) * 20f;
            masterMixer.SetFloat("MusicVolume", volumeInDb);
            yield return null;
        }

        masterMixer.SetFloat("MusicVolume", -80f);
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

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
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

        if (pauseMenuController != null)
        {
            if (isPaused)
                pauseMenuController.ShowPauseMenu();
            else
                pauseMenuController.HidePauseMenu();
        }
        else
        {
            Debug.LogWarning("PauseMenuController is missing.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CloseSettings()
    {
        CurrentState = GameState.Title;
        SceneManager.LoadScene("Title");
    }
}
