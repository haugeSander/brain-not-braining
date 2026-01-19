using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            SoundManager.instance.PlaySound(PressureClip);
            
            SoundManager.instance.PlaySound(DoorOpeningClip);   
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
        SoundManager.instance.PlaySound(DoorOpeningClip);
        yield return new WaitForSeconds(DoorOpeningClip.length);
        
        SceneManager.LoadScene("LevelFinished");
    }
}