using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeToBlack(float duration, string sceneName)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        // Fully black
        canvasGroup.alpha = 1;

        // Load next scene
        SceneManager.LoadScene(sceneName);
    }
}
