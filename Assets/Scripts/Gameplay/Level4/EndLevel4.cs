using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLeve4 : MonoBehaviour
{
    public TextMeshProUGUI text;
    public AudioClip DoorOpeningClip;

    public void Start()
    {
        text.enabled = false;
    }

    public void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            text.text="Finished level 4... ";
            text.enabled= true;
             StartCoroutine("EndLevel");
            // SoundManager.instance.PlaySound(HitClip);   
        } 
    }

      void OnCollisionExit(Collision other)
    {
        text.enabled = false;
      
    }

     IEnumerator EndLevel()
    {
        Debug.Log("Level Complete!");
        SoundManager.instance.PlaySound(DoorOpeningClip);
        yield return new WaitForSeconds(DoorOpeningClip.length);
        
        SceneManager.LoadScene("LevelFinished");
    }
}