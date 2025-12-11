using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _Instance;
    public static SoundManager instance{get{return _Instance;}}
    private AudioSource source;
    private void Awake()
    {

       
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _Instance = this;
        }
        
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip audioClip)
    {
        source.PlayOneShot(audioClip);
    }


    public void StopSound()
    {
        source.Stop();
    }

}