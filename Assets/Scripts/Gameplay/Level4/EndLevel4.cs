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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !levelCompleted)
        {
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
        Debug.Log("Level 4 completed! Loading brain region unlock scene...");


        // Set which region should be unlocked (Level 4 unlocks VisualCortex)
        ProgressionManager.PendingUnlock = BrainRegion.VisualCortex;

        SceneManager.LoadScene(BRAIN_REGION_SCENE);
        Debug.Log("Level Complete!");
        SoundManager.instance.PlaySound(DoorOpeningClip);
        yield return new WaitForSeconds(DoorOpeningClip.length);
        
        SceneManager.LoadScene("LevelFinished");
    }
}
