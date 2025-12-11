using UnityEngine;

public class WallBehaviour: MonoBehaviour
{
   
    public AudioSource audioSource;
    public AudioClip audioClip;


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name + " from : " +gameObject.name);
        audioSource.PlayOneShot(audioClip); 
        
    }

     void OnParticleCollision(GameObject other)
    {
        audioSource.PlayOneShot(audioClip); 
    }
}