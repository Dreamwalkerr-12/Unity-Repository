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
        if (CurrentState == GameState.GameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            LoadTitle();
        }
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        SceneManager.LoadScene("GameOver");
    }

    public void LoadTitle()
    {
        CurrentState = GameState.Title;
        SceneManager.LoadScene("Title");
    }

    public void ResetGame()
    {
        CurrentState = GameState.Playing;
        SceneManager.LoadScene("Game");
    }
}
