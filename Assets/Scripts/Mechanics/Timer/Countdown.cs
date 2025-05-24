using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 300f; // 5 minutes
    public TextMeshProUGUI timerText; // Assign in Inspector

    private bool timerRunning = true;
    private PlayerController player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>(); // Get PlayerController from scene
        UpdateTimerUI();
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;
                UpdateTimerUI();
                HandleTimeOut();
            }

            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = seconds.ToString();
    }

    void HandleTimeOut()
    {
        if (player != null)
        {
            player.Lives -= 1;

            // Optionally restart timer if player has lives left
            if (player.Lives > 0)
            {
                timeRemaining = 300f;
                timerRunning = true;
                UpdateTimerUI();
            }
        }
    }
}
