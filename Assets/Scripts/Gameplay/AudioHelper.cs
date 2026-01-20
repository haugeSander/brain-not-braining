using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Simple audio helper that routes all sounds through the Audio Mixer.
/// 
/// Usage:
///   AudioSource.PlayClipAtPoint(myClip, transform.position);
///   becomes AudioHelper.PlayMusic(myClip, transform.position);
/// </summary>
public class AudioHelper : MonoBehaviour
{
    private static AudioHelper _instance;
    public static AudioHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AudioHelper");
                _instance = go.AddComponent<AudioHelper>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Audio Mixer Groups")]
    [Tooltip("Assign your SFX mixer group here")]
    public AudioMixerGroup sfxMixerGroup;
    
    [Tooltip("Assign your Music mixer group here")]
    public AudioMixerGroup musicMixerGroup;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play a sound effect through the SFX mixer group.
    /// Drop-in replacement for AudioHelper.PlaySFX.
    /// </summary>
    public static void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        PlayClipThroughMixer(clip, position, volume, Instance.sfxMixerGroup);
    }

    /// <summary>
    /// Play music through the Music mixer group.
    /// </summary>
    public static void PlayMusic(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        PlayClipThroughMixer(clip, position, volume, Instance.musicMixerGroup);
    }

    /// <summary>
    /// Internal method that creates a temporary AudioSource with proper mixer routing.
    /// </summary>
    private static void PlayClipThroughMixer(AudioClip clip, Vector3 position, float volume, AudioMixerGroup mixerGroup)
    {
        GameObject tempGO = new GameObject($"TempAudio_{clip.name}");
        tempGO.transform.position = position;
        
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1.0f; // 3D sound
        source.outputAudioMixerGroup = mixerGroup; // Route through mixer!
        source.Play();
        
        // Destroy after clip finishes
        Destroy(tempGO, clip.length + 0.1f);
    }

    /// <summary>
    /// Play a 2D sound effect (non-spatial).
    /// </summary>
    public static void PlaySFX2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        PlayClipThroughMixer2D(clip, volume, Instance.sfxMixerGroup);
    }

    /// <summary>
    /// Play 2D music (non-spatial).
    /// </summary>
    public static void PlayMusic2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        PlayClipThroughMixer2D(clip, volume, Instance.musicMixerGroup);
    }

    private static void PlayClipThroughMixer2D(AudioClip clip, float volume, AudioMixerGroup mixerGroup)
    {
        GameObject tempGO = new GameObject($"TempAudio2D_{clip.name}");
        
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f; // 2D sound
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();
        
        Destroy(tempGO, clip.length + 0.1f);
    }
}