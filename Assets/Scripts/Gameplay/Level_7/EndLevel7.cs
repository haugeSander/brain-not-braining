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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            text.enabled= true;
            
            // SoundManager.instance.PlaySound(HitClip);   
        } 
    }

    void OnCollisionStay2D(Collision2D collision)
    {
            if (collision.gameObject.tag == "Player" && Input.GetKeyDown(KeyCode.F))
        {
            
            StartCoroutine("EndLevel");
            
            // SoundManager.instance.PlaySound(HitClip);   
        }  
    }

    void OnCollisionExit2D(Collision2D other)
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