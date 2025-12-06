using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    public AudioClip HitClip;
    public AudioClip UsingClip;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

   void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
         if(collision.gameObject.tag=="Player")
            SoundManager.instance.PlaySound(HitClip);    
    }
}