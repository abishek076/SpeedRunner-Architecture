using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static float CurrentSpeed = 10f; 
    private float distanceInterval = 500f;
    private float speedIncrement = 2f;
    private float nextThreshold = 500f;

    [Header("Distance & Progression")]
    public float currentDistance = 0f;
    
    [Header("UI Elements")]
    public TextMeshProUGUI distanceText; // The physical text on the screen
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;

    [Header("Game State")]
    public static bool isGameOver = false; // A global flag other scripts can read
    private float distance;


    void Start()
    {
        CurrentSpeed = 10f; 
        currentDistance = 0f;
        nextThreshold = distanceInterval;
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        distanceText.gameObject.SetActive(false);
        isGameOver = true;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isGameOver) return;
        // 1. Calculate the distance mathematically based on the treadmill speed
        currentDistance += CurrentSpeed * Time.deltaTime;

        // 2. Update the UI text
        if (distanceText != null)
        {
            // .ToString("F0") strips away all the messy decimals so it displays cleanly (e.g., "432m")
            distanceText.text = currentDistance.ToString("F0") + "m";
        }

        // 3. Check for the difficulty speed increase
        if (currentDistance >= nextThreshold)
        {
            IncreaseDifficulty();
        }
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        distanceText.gameObject.SetActive(true);
        isGameOver = false;
        Time.timeScale = 1f; // Unfreeze the engine
    }

    private void IncreaseDifficulty()
    {
        CurrentSpeed += speedIncrement; 
        nextThreshold += distanceInterval; 
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}