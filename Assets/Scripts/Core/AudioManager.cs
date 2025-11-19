using UnityEngine;

namespace BrainNotBraining.Core
{
    /// <summary>
    /// Manages all audio in the game with progressive layering.
    /// Level 0: Heartbeat only
    /// Level 1+: Add footsteps, ambient, music layers
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        // === SINGLETON PATTERN ===
        public static AudioManager Instance { get; private set; }

        // === AUDIO SOURCES ===
        [Header("Audio Sources")]
        [Tooltip("AudioSource for the heartbeat loop (Level 0)")]
        public AudioSource heartbeatSource;

        [Tooltip("AudioSource for footsteps (Level 1+)")]
        public AudioSource footstepsSource;

        [Tooltip("AudioSource for ambient sounds")]
        public AudioSource ambientSource;

        [Tooltip("AudioSource for music")]
        public AudioSource musicSource;

        // === AUDIO CLIPS ===
        [Header("Audio Clips")]
        [Tooltip("Heartbeat loop clip - drag your audio file here in Inspector")]
        public AudioClip heartbeatClip;

        // Future: footstep clips, ambient clips, music clips

        // === UNITY LIFECYCLE ===

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeAudioSources();
        }

        /// <summary>
        /// Creates AudioSource components if they don't exist (programmatically).
        /// </summary>
        private void InitializeAudioSources()
        {
            // Try to load heartbeat clip from Resources if not assigned
            if (heartbeatClip == null)
            {
                heartbeatClip = Resources.Load<AudioClip>("Audio/heart-beat");
                if (heartbeatClip == null)
                {
                    Debug.LogWarning("AudioManager: Could not load heartbeat clip. Please assign it in Inspector or place it in Resources/Audio/heart-beat");
                }
            }

            if (heartbeatSource == null)
            {
                heartbeatSource = gameObject.AddComponent<AudioSource>();
            }
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.playOnAwake = false;
            heartbeatSource.volume = 0.7f;

            if (footstepsSource == null)
            {
                footstepsSource = gameObject.AddComponent<AudioSource>();
            }
            footstepsSource.clip = null;
            footstepsSource.loop = false;
            footstepsSource.playOnAwake = false;
            footstepsSource.volume = 0.0f;

            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
            }
            ambientSource.clip = null;
            ambientSource.loop = false;
            ambientSource.playOnAwake = false;
            ambientSource.volume = 0.0f;

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            musicSource.clip = null;
            musicSource.loop = false;
            musicSource.playOnAwake = false;
            musicSource.volume = 0.0f;

            Debug.Log("AudioManager initialized");
        }

        // === PUBLIC API ===

        /// <summary>
        /// Starts playing the heartbeat (called when Level 0 begins).
        /// </summary>
        public void PlayHeartbeat()
        {
            if (heartbeatSource != null && heartbeatSource.clip != null)
            {
                heartbeatSource.Play();
                Debug.Log("Heartbeat sound started");
            }
            else
            {
                Debug.Log("ERROR: No Heartbeat sound source found!");
            }
        }

        /// <summary>
        /// Stops the heartbeat (if needed for transitions).
        /// </summary>
        public void StopHeartbeat()
        {
            if (heartbeatSource != null && heartbeatSource.clip != null)
            {
                heartbeatSource.Stop();
                Debug.Log("Heartbeat sound stopped");
            }
            else
            {
                Debug.Log("ERROR: No heartbeat source found!");
            }
        }

        /// <summary>
        /// Plays a one-shot sound effect (e.g., button click, door open).
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1.0f)
        {
            if (clip != null)
            {
                heartbeatSource.PlayOneShot(clip, volume);
            }
        }

        // === FUTURE: PROGRESSIVE LAYERING ===
        // These methods will be implemented in later levels

        public void EnableFootsteps()
        {
            // TODO (Level 1): Start playing footstep sounds
            // Will be triggered when MotorCortex is unlocked
        }

        public void EnableAmbient()
        {
            // TODO (Level 2): Start playing ambient sounds
        }

        public void EnableMusic()
        {
            // TODO (Level 3+): Start playing background music
        }
    }
}
