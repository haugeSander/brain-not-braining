using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLeve4 : MonoBehaviour
{
    bool inBasket = false;
    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inBasket= true;
            StartCoroutine("EndLevel");
            // SoundManager.instance.PlaySound(HitClip);   
        } 
    }

      void OnCollisionExit2D(Collision2D other)
    {
        inBasket = false;
        StopCoroutine("EndLevel");
    }

     void EndLevel()
    {
        Debug.Log("Level Complete!");
        // yield WaitForSeconds(3);
        if (inBasket)
            SceneManager.LoadScene("LevelFinished");
    }
}