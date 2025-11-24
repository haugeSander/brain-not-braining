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

        [Tooltip("AudioSource for sound effects (dedicated channel)")]
        public AudioSource sfxSource;

        // === AUDIO CLIPS ===
        [Header("Audio Clips")]
        [Tooltip("Heartbeat loop clip - drag your audio file here in Inspector")]
        public AudioClip heartbeatClip;

        [Header("SFX Clips")]
        [Tooltip("Button click success sound")]
        public AudioClip buttonClickClip;

        [Tooltip("Button miss/timeout sound")]
        public AudioClip buttonMissClip;

        [Tooltip("Breath inhale sound")]
        public AudioClip breathInhaleClip;

        [Tooltip("Breath exhale sound")]
        public AudioClip breathExhaleClip;

        [Tooltip("Irregular breath warning")]
        public AudioClip breathWarningClip;

        [Tooltip("HR milestone reached")]
        public AudioClip hrMilestoneClip;

        [Tooltip("Ambient music")]
        public AudioClip musicClip;


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
        /// Note: If clips aren't assigned in Inspector, you must manually assign them.
        /// Resources.Load is disabled - use Inspector assignment instead.
        /// </summary>
        private void InitializeAudioSources()
        {
            // Log which clips are missing (must be assigned in Inspector)
            if (heartbeatClip == null)
            {
                Debug.LogWarning("AudioManager: Heartbeat clip not assigned! Drag from Assets/Audio/ to AudioManager in Inspector");
            }

            if (buttonClickClip == null)
            {
                Debug.LogWarning("AudioManager: button_click not assigned! Drag from Assets/Audio/SFX/ to AudioManager");
            }

            if (buttonMissClip == null)
            {
                Debug.LogWarning("AudioManager: button_miss not assigned! Drag from Assets/Audio/SFX/ to AudioManager");
            }

            if (breathInhaleClip == null)
            {
                Debug.LogWarning("AudioManager: breath_inhale not assigned! Drag from Assets/Audio/SFX/ to AudioManager");
            }

            if (breathExhaleClip == null)
            {
                Debug.LogWarning("AudioManager: breath_exhale not assigned! Drag from Assets/Audio/SFX/ to AudioManager");
            }

            if (breathWarningClip == null)
            {
                Debug.LogWarning("AudioManager: breath_warning not assigned! Drag from Assets/Audio/SFX/ to AudioManager");
            }

            if (hrMilestoneClip == null)
            {
                Debug.LogWarning("AudioManager: hr_milestone not assigned! Drag from Assets/Audio/SFX/ to AudioManager");
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

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            sfxSource.clip = null;
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = 1.0f;

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
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }

        // === PUZZLE 0 SFX HELPERS ===

        public void PlayButtonClick()
        {
            if (buttonClickClip != null)
            {
                PlaySFX(buttonClickClip, 0.8f);
            }
        }

        public void PlayButtonMiss()
        {
            if (buttonMissClip != null)
            {
                PlaySFX(buttonMissClip, 0.7f);
            }
        }

        public void PlayBreathInhale()
        {
            if (breathInhaleClip != null)
            {
                PlaySFX(breathInhaleClip, 0.5f);
            }
        }

        public void PlayBreathExhale()
        {
            if (breathExhaleClip != null)
            {
                PlaySFX(breathExhaleClip, 0.5f);
            }
        }

        public void PlayBreathWarning()
        {
            if (breathWarningClip != null)
            {
                PlaySFX(breathWarningClip, 0.9f);
            }
        }

        public void PlayHRMilestone()
        {
            if (hrMilestoneClip != null)
            {
                PlaySFX(hrMilestoneClip, 0.8f);
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
            // Music must be assigned to musicSource.clip in Inspector
            // Or assign via ReflexBreathPuzzle's musicSource field
            if (musicSource != null && musicSource.clip != null)
            {
                musicSource.loop = true;
                musicSource.volume = 0.3f; // Start quiet
                musicSource.Play();
                Debug.Log("Music started: " + musicSource.clip.name);
            }
            else
            {
                Debug.LogWarning("Music not assigned! Drag audio file to AudioManager's musicSource or assign in ReflexBreathPuzzle");
            }
        }
    }
}
