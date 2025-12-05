using UnityEngine;

public class BoxBehaviour: MonoBehaviour
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