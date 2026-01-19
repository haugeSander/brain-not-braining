using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel3 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    bool inBasket = false;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Entered collision with " + collision.gameObject.name);
        inBasket = true;
        StartCoroutine("EndLevel");
    }

    void OnCollisionExit(Collision other)
    {
        Debug.Log("Player exited the basket area");
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
