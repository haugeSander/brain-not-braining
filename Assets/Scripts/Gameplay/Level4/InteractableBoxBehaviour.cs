using TMPro;
using UnityEngine;

public class InteractableBoxBehaviour : MonoBehaviour
{
    public AudioClip HitClip;
    public AudioClip UsingClip;

    public TextMeshProUGUI text;

    private void Start()
    {
        text.enabled=false;
    }

    private void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Player")
        {
            text.enabled=true;
            SoundManager.instance.PlaySound(HitClip);
        
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        text.enabled = false;
    }
}