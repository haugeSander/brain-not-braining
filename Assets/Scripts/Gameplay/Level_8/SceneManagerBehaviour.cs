using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerBehaviour : MonoBehaviour
{


    float timeLeft = 180;
    public TextMeshProUGUI countdownText;

    void Update()
    {

        timeLeft -= Time.deltaTime;
        if(countdownText != null)
            countdownText.text = ((int)timeLeft).ToString();
        if (timeLeft < 30.0f)
        {
            countdownText.color = Color.red;
        }
        if (timeLeft < 0)
        {
            if(countdownText != null)
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