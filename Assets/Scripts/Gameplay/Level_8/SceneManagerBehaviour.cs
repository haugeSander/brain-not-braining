using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerBehaviour : MonoBehaviour
{
    float timeLeft = 180;
    public TextMeshProUGUI countdownText;

    void OnEnable()
    {
        // Lock cursor when scene becomes active
        StartCoroutine(LockCursorAfterDelay());
    }

    IEnumerator LockCursorAfterDelay()
    {
        // Wait one frame to ensure scene is fully loaded
        yield return null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked by SceneManagerBehaviour");
    }


    void Update()
    {
        timeLeft -= Time.deltaTime;
        countdownText.text = ((int)timeLeft).ToString();
        if (timeLeft < 30.0f)
        {
            countdownText.color = Color.red;
        }
        if (timeLeft < 0)
        {
            countdownText.text = "YOU LOSE!";

            StartCoroutine(GameOver());
        }
    }
    // to use the menu for restarting level
    IEnumerator GameOver()
    {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("LevelFinished");
    }
}