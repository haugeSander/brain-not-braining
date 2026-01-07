using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

public class EndLevel4 : MonoBehaviour
{
    private const string BRAIN_REGION_SCENE = "BrainRegionUnlocked";

    public TextMeshProUGUI text;
    public AudioClip DoorOpeningClip;
    private bool levelCompleted = false;

    public void Start()
    {
        if (text != null)
            text.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !levelCompleted)
        {
            levelCompleted = true;
            text.text="Finished level 4... ";
            text.enabled= true;
            StartCoroutine("EndLevel");
            // SoundManager.instance.PlaySound(HitClip);
        }
    }

      void OnCollisionExit(Collision other)
    {
        if (text != null && !levelCompleted)
            text.enabled = false;
    }

    IEnumerator EndLevel()
    {
        Debug.Log("Level 4 completed! Playing completion sound...");

        // Play completion sound
        SoundManager.instance.PlaySound(DoorOpeningClip);

        // Wait for audio to finish
        yield return new WaitForSeconds(DoorOpeningClip.length);

        // Set which region should be unlocked (Level 4 unlocks VisualCortex)
        ProgressionManager.PendingUnlock = BrainRegion.VisualCortex;

        Debug.Log("Loading brain region unlock scene...");
        SceneManager.LoadScene(BRAIN_REGION_SCENE);
    }
}
