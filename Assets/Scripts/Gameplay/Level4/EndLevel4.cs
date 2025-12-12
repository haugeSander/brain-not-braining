using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

public class EndLeve4 : MonoBehaviour
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !levelCompleted)
        {
            if (text != null)
                text.enabled = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F) && !levelCompleted)
        {
            levelCompleted = true;
            StartCoroutine(EndLevel());
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (text != null && !levelCompleted)
            text.enabled = false;
    }

    IEnumerator EndLevel()
    {
        Debug.Log("Level 4 completed! Loading brain region unlock scene...");

        if (SoundManager.instance != null && DoorOpeningClip != null)
            SoundManager.instance.PlaySound(DoorOpeningClip);

        yield return new WaitForSeconds(4);

        // Set which region should be unlocked (Level 4 unlocks VisualCortex)
        ProgressionManager.PendingUnlock = BrainRegion.VisualCortex;

        SceneManager.LoadScene(BRAIN_REGION_SCENE);
    }
}