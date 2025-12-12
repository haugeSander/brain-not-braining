using UnityEngine;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    public string nextSceneName;
    public float fadeDuration = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ScreenFader.Instance.FadeToBlack(fadeDuration, nextSceneName));
        }
    }
}

