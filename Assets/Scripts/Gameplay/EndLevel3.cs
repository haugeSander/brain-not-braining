using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using BrainNotBraining.Core;

public class EndLevel3 : MonoBehaviour
{
    private const string BRAIN_REGION_SCENE = "BrainRegionUnlocked";
    private bool inBasket = false;
    private bool levelCompleted = false;

    void OnCollisionEnter(Collision collision)
    {
        if (levelCompleted) return;

        Debug.Log("Entered collision with " + collision.gameObject.name);
        inBasket = true;
        StartCoroutine(EndLevelCoroutine());
    }

    void OnCollisionExit(Collision other)
    {
        if (levelCompleted) return;

        Debug.Log("Player exited the basket area");
        inBasket = false;
        StopCoroutine(EndLevelCoroutine());
    }

    IEnumerator EndLevelCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // Brief delay to ensure stable collision

        if (inBasket && !levelCompleted)
        {
            levelCompleted = true;
            Debug.Log("Level 3 completed! Loading brain region unlock scene...");

            // Set which region should be unlocked (Level 3 unlocks Somatosensory)
            ProgressionManager.PendingUnlock = BrainRegion.Somatosensory;

            SceneManager.LoadScene(BRAIN_REGION_SCENE);
        }
    }
}
