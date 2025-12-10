using UnityEngine;

public class BoxBehaviour2D : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}