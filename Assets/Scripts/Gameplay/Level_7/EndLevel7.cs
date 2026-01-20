using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

public class EndLeve7 : MonoBehaviour
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

            if (text != null)
            {
                text.text = "Level finished continuing...";
                text.enabled = true;
            }

            StartCoroutine(EndLevel());
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (text != null && !levelCompleted)
            text.enabled = false;
    }

    IEnumerator EndLevel()
    {
        Debug.Log("Level 7 (Prefrontal) completed! Loading brain region unlock scene...");

        yield return new WaitForSeconds(4);

        // Set which region should be unlocked (Level 7 unlocks Prefrontal)
        ProgressionManager.PendingUnlock = BrainRegion.Prefrontal;

        SceneManager.LoadScene(BRAIN_REGION_SCENE);
    }
}