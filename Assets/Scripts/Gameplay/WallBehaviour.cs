using UnityEngine;

public class WallBehaviour: MonoBehaviour
{
   
    public AudioSource audioSource;
    public AudioClip audioClip;


    void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(audioClip); 
        
    }

     void OnParticleCollision(GameObject other)
    {
        audioSource.PlayOneShot(audioClip); 
    }
}