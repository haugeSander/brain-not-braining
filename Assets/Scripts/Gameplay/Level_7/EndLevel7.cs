using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLeve7 : MonoBehaviour
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
             text.text="Level finished continuing...";
            text.enabled= true;
            
            SoundManager.instance.PlaySound(DoorOpeningClip);   
            StartCoroutine("EndLevel");
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
        yield return new WaitForSeconds(4);
        
        SceneManager.LoadScene("LevelFinished");
    }
}