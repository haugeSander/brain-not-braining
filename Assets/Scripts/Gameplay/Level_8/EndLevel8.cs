using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

public class EndLevel8 : MonoBehaviour
{
    public TextMeshProUGUI text;
    public AudioClip DoorOpeningClip;
    public AudioClip PressureClip;
    bool isLoading= false;
    public void Start()
    {
       
        text.enabled = false;
    }

    public void Update()
    {
        
    }



     void OnTriggerEnter(Collider collision)
    {
  
        if (collision.gameObject.tag == "Player" && !isLoading)
        {
             text.text="Level finished continuing...";
            text.enabled= true;

            AudioHelper.PlaySFX(PressureClip, transform.position, 1f);
            
            AudioHelper.PlaySFX(DoorOpeningClip, transform.position, 1f);
            StartCoroutine("EndLevel");
        } 
    }

    void OnTriggerExit(Collider collision)
    {
        text.enabled = false;
    }

    



     IEnumerator EndLevel()
    {
        isLoading=true;
        Debug.Log("Level Complete!");
        AudioHelper.PlaySFX(DoorOpeningClip, transform.position, 1f);
        yield return new WaitForSeconds(DoorOpeningClip.length);

        ProgressionManager.PendingUnlock = BrainRegion.Full;
        
        SceneManager.LoadScene("EndSequence");
    }
}