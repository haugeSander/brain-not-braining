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
            heartbeatSource.volume = 0.4f; // Lowered from 0.7f

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

            audioSourcesInitialized = true;
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
                PlaySFX(buttonClickClip, 0.3f);
            }
        }

        public void PlayButtonMiss()
        {
            if (buttonMissClip != null)
            {
                PlaySFX(buttonMissClip, 0.3f);
            }
        }

        public void PlayBreathInhale()
        {
            if (breathInhaleClip != null)
            {
                PlaySFX(breathInhaleClip, 0.2f);
            }
        }

        public void PlayBreathExhale()
        {
            if (breathExhaleClip != null)
            {
                PlaySFX(breathExhaleClip, 0.2f);
            }
        }

        public void PlayBreathWarning()
        {
            if (breathWarningClip != null)
            {
                PlaySFX(breathWarningClip, 0.4f);
            }
        }

        public void PlayHRMilestone()
        {
            if (hrMilestoneClip != null)
            {
                PlaySFX(hrMilestoneClip, 0.3f);
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

        // === AUDIO FADE SYSTEM (FOR WIN/LOSE CONDITIONS) ===

        private AudioLowPassFilter musicLowPass;
        private AudioLowPassFilter heartbeatLowPass;
        private bool isMuffling = false;
        private float muffleProgress = 0f;
        private bool audioSourcesInitialized = false;

        /// <summary>
        /// Start muffling audio (apply lowpass filter gradually).
        /// Used for low HR warning and idle breathing warning.
        /// </summary>
        public void StartMuffling(float duration = 2f)
        {
            if (isMuffling) return; // Already muffling

            // Ensure audio sources are initialized first
            if (!audioSourcesInitialized)
            {
                Debug.LogWarning("AudioManager: Audio sources not initialized yet, cannot start muffling");
                return;
            }

            // Add lowpass filters if they don't exist
            if (musicSource != null && musicSource.gameObject != null && musicLowPass == null)
            {
                try
                {
                    musicLowPass = musicSource.gameObject.AddComponent<AudioLowPassFilter>();
                    musicLowPass.cutoffFrequency = 22000f; // Start at max (no filtering)
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"AudioManager: Could not add lowpass to music source: {e.Message}");
                }
            }

            if (heartbeatSource != null && heartbeatSource.gameObject != null && heartbeatLowPass == null)
            {
                try
                {
                    heartbeatLowPass = heartbeatSource.gameObject.AddComponent<AudioLowPassFilter>();
                    heartbeatLowPass.cutoffFrequency = 22000f; // Start at max (no filtering)
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"AudioManager: Could not add lowpass to heartbeat source: {e.Message}");
                }
            }

            // Only start muffling if we have at least one filter
            if (musicLowPass != null || heartbeatLowPass != null)
            {
                isMuffling = true;
                muffleProgress = 0f;
                StartCoroutine(MuffleAudioCoroutine(duration));
                Debug.Log("AudioManager: Starting audio muffling");
            }
            else
            {
                Debug.LogWarning("AudioManager: No audio sources available for muffling");
            }
        }

        /// <summary>
        /// Stop muffling and restore clear audio.
        /// Called when player recovers from danger state.
        /// </summary>
        public void StopMuffling(float duration = 1f)
        {
            if (!isMuffling && musicLowPass == null && heartbeatLowPass == null) return;

            isMuffling = false;

            // Only start unmuffling if we have filters to remove
            if (musicLowPass != null || heartbeatLowPass != null)
            {
                StartCoroutine(UnmuffleAudioCoroutine(duration));
                Debug.Log("AudioManager: Stopping audio muffling");
            }
        }

        private System.Collections.IEnumerator MuffleAudioCoroutine(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration && isMuffling)
            {
                elapsed += Time.unscaledDeltaTime;
                muffleProgress = Mathf.Clamp01(elapsed / duration);

                // Gradually reduce cutoff frequency (22000 Hz → 500 Hz)
                float targetCutoff = Mathf.Lerp(22000f, 500f, muffleProgress);

                if (musicLowPass != null)
                    musicLowPass.cutoffFrequency = targetCutoff;

                if (heartbeatLowPass != null)
                    heartbeatLowPass.cutoffFrequency = targetCutoff;

                yield return null;
            }

            Debug.Log("AudioManager: Muffling complete");
        }

        private System.Collections.IEnumerator UnmuffleAudioCoroutine(float duration)
        {
            float elapsed = 0f;
            float startCutoff = musicLowPass != null ? musicLowPass.cutoffFrequency : 500f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Gradually restore cutoff frequency (current → 22000 Hz)
                float targetCutoff = Mathf.Lerp(startCutoff, 22000f, t);

                if (musicLowPass != null)
                    musicLowPass.cutoffFrequency = targetCutoff;

                if (heartbeatLowPass != null)
                    heartbeatLowPass.cutoffFrequency = targetCutoff;

                yield return null;
            }

            // Remove lowpass filters when done
            if (musicLowPass != null)
            {
                Destroy(musicLowPass);
                musicLowPass = null;
            }

            if (heartbeatLowPass != null)
            {
                Destroy(heartbeatLowPass);
                heartbeatLowPass = null;
            }

            muffleProgress = 0f;
            Debug.Log("AudioManager: Unmuffling complete");
        }

        /// <summary>
        /// Gradually fade out all audio sources except heartbeat over specified duration.
        /// Used for lose condition atmospheric effect.
        /// </summary>
        public void FadeOutAllExceptHeartbeat(float duration)
        {
            StartCoroutine(FadeOutAllExceptHeartbeatCoroutine(duration));
        }

        private System.Collections.IEnumerator FadeOutAllExceptHeartbeatCoroutine(float duration)
        {
            float elapsed = 0f;
            float startMusicVolume = musicSource != null ? musicSource.volume : 0f;
            float startFootstepsVolume = footstepsSource != null ? footstepsSource.volume : 0f;
            float startAmbientVolume = ambientSource != null ? ambientSource.volume : 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled time in case game is paused
                float t = elapsed / duration;

                // Fade out music, footsteps, ambient (but NOT heartbeat or SFX)
                if (musicSource != null)
                    musicSource.volume = Mathf.Lerp(startMusicVolume, 0f, t);

                if (footstepsSource != null)
                    footstepsSource.volume = Mathf.Lerp(startFootstepsVolume, 0f, t);

                if (ambientSource != null)
                    ambientSource.volume = Mathf.Lerp(startAmbientVolume, 0f, t);

                yield return null;
            }

            // Ensure fully muted
            if (musicSource != null) musicSource.volume = 0f;
            if (footstepsSource != null) footstepsSource.volume = 0f;
            if (ambientSource != null) ambientSource.volume = 0f;

            Debug.Log("AudioManager: All audio faded out except heartbeat");
        }

        /// <summary>
        /// Add echo effect to heartbeat and slowly fade it out.
        /// Creates atmospheric "consciousness fading" effect.
        /// </summary>
        public void EchoHeartbeat()
        {
            StartCoroutine(EchoHeartbeatCoroutine());
        }

        private System.Collections.IEnumerator EchoHeartbeatCoroutine()
        {
            if (heartbeatSource == null) yield break;

            float startVolume = heartbeatSource.volume;
            float startPitch = heartbeatSource.pitch;
            float duration = 3f;
            float elapsed = 0f;

            // Gradually slow down and fade out heartbeat
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Slow down pitch (simulating dying heartbeat)
                heartbeatSource.pitch = Mathf.Lerp(startPitch, startPitch * 0.5f, t);

                // Fade out volume
                heartbeatSource.volume = Mathf.Lerp(startVolume, 0f, t);

                yield return null;
            }

            heartbeatSource.volume = 0f;
            Debug.Log("AudioManager: Heartbeat echo complete");
        }

        /// <summary>
        /// Restore all audio to normal volumes (for retry/restart).
        /// </summary>
        public void RestoreAudio()
        {
            // Restore default volumes
            if (heartbeatSource != null)
            {
                heartbeatSource.volume = 0.4f; // Lowered from 0.7f
                heartbeatSource.pitch = 0.8f; // Reset to starting pitch (raised from 0.4)
            }

            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.volume = 0.3f; // Or whatever default you want
                musicSource.pitch = 1.0f; // Ensure music is never pitched
            }

            if (footstepsSource != null)
                footstepsSource.volume = 0.0f; // Default off

            if (ambientSource != null)
                ambientSource.volume = 0.0f; // Default off

            Debug.Log("AudioManager: Audio restored to default volumes");
        }
    }
}
