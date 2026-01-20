using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;
using BrainNotBraining.Gameplay;

public class TestPathEndTrigger : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    public string BRAIN_REGION_SCENE = "BrainRegionUnlocked";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level 1 completed! Loading brain region unlock scene...");

            // Set which region should be unlocked (Level 1 unlocks MotorCortex)
            ProgressionManager.PendingUnlock = BrainRegion.Prefrontal;

            // Use ScreenFader if available, otherwise load directly
            if (ScreenFader.Instance != null)
            {
                StartCoroutine(ScreenFader.Instance.FadeToBlack(fadeDuration, BRAIN_REGION_SCENE));
            }
            else
            {
                SceneManager.LoadScene(BRAIN_REGION_SCENE);
            }
        }
    }
}

