using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // needed to reload the scene
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeLeft = 60f; // countdown in seconds
    public TMP_Text timerText;       // drag your UI Text here
    public GameObject restartButton; // drag your restart button here
    public TMP_Text startText;  

    private bool gameRunning = false;

    void Start()
    {
        restartButton.SetActive(false);  
        startText.gameObject.SetActive(true);  // show start text
        timerText.text = Mathf.Ceil(timeLeft).ToString();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;  
    }

    void Update()
    {
        // Start the game when any key or mouse button is pressed
        if (!gameRunning && Input.anyKeyDown)
        {
            StartGame();
        }

        // Countdown timer
        if (gameRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                EndGame();
            }
            UpdateTimerUI();
        }
    }

    void StartGame()
    {
        gameRunning = true;
        startText.gameObject.SetActive(false); // hide start text
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;                  // resume game
    }

    void UpdateTimerUI()
    {
        timerText.text = Mathf.Ceil(timeLeft).ToString();
    }

    void EndGame()
    {
        gameRunning = false;
        restartButton.SetActive(true);
        timerText.text = "Game Over!";
        Time.timeScale = 0f;              // pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;            // show cursor so you can click the button
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // resume time before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload the current scene
    }
}
