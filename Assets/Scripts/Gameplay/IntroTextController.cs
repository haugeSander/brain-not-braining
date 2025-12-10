using UnityEngine;
using TMPro;

public class IntroTextController : MonoBehaviour
{
    public GameObject introPanel;

    void Start()
    {
        Time.timeScale = 0f; // pause game
        introPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            introPanel.SetActive(false);
            Time.timeScale = 1f; // unpause game
            Destroy(gameObject); // optional cleanup
        }
    }
}
