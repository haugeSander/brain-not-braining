using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

/// <summary>
/// Manages game settings including audio, graphics, and controls.
/// Saves settings to PlayerPrefs for persistence across sessions.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Toggle invertYToggle;

    [Header("Developer Settings")]
    [SerializeField] private Toggle cheatsToggle;
    [Tooltip("If true, cheats section is visible in settings. Set to false for release builds.")]
    public bool showCheatsInSettings = true;

    [Header("UI References (Optional)")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private TextMeshProUGUI sensitivityText;

    // PlayerPrefs keys
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string QUALITY_KEY = "QualityLevel";
    private const string VSYNC_KEY = "VSync";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string SENSITIVITY_KEY = "MouseSensitivity";
    private const string INVERT_Y_KEY = "InvertY";
    private const string CHEATS_KEY = "CheatsEnabled";

    // Public property for other scripts to access sensitivity
    public static float MouseSensitivity { get; private set; } = 2.0f;
    public static bool InvertY { get; private set; } = false;
    public static bool CheatsEnabled { get; private set; } = true;

    private void Start()
    {
        LoadSettings();
        SetupListeners();
        
        // Hide cheats toggle if showCheatsInSettings is false
        if (cheatsToggle != null && !showCheatsInSettings)
        {
            cheatsToggle.transform.parent.gameObject.SetActive(false);
        }
    }

    private void SetupListeners()
    {
        // Audio listeners
        if (masterVolumeSlider != null) 
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null) 
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null) 
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Graphics listeners
        if (qualityDropdown != null) 
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        if (vsyncToggle != null) 
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        if (fullscreenToggle != null) 
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

        // Gameplay listeners
        if (mouseSensitivitySlider != null) 
            mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        if (invertYToggle != null) 
            invertYToggle.onValueChanged.AddListener(OnInvertYChanged);
        
        // Developer listeners
        if (cheatsToggle != null)
            cheatsToggle.onValueChanged.AddListener(OnCheatsChanged);
    }

    private void LoadSettings()
    {
        // Load Audio Settings
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.8f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVol;
        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVol;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVol;

        // Apply to Audio Mixer
        SetMixerVolume("MasterVolume", masterVol);
        SetMixerVolume("MusicVolume", musicVol);
        SetMixerVolume("SFXVolume", sfxVol);

        // Load Graphics Settings
        int quality = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
        bool vsync = PlayerPrefs.GetInt(VSYNC_KEY, 1) == 1;
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0) == 1;

        if (qualityDropdown != null) qualityDropdown.value = quality;
        QualitySettings.SetQualityLevel(quality);
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        if (vsyncToggle != null) vsyncToggle.isOn = vsync;
        Screen.fullScreen = fullscreen;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

        // Load Gameplay Settings
        MouseSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 2.0f);
        InvertY = PlayerPrefs.GetInt(INVERT_Y_KEY, 0) == 1;
        CheatsEnabled = PlayerPrefs.GetInt(CHEATS_KEY, 0) == 1;

        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = MouseSensitivity;
        if (invertYToggle != null) invertYToggle.isOn = InvertY;
        if (cheatsToggle != null) cheatsToggle.isOn = CheatsEnabled;

        UpdateAllTexts();
    }

    // === AUDIO CALLBACKS ===
    private void OnMasterVolumeChanged(float value)
    {
        SetMixerVolume("MasterVolume", value);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
        if (masterVolumeText != null) 
            masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void OnMusicVolumeChanged(float value)
    {
        SetMixerVolume("MusicVolume", value);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        if (musicVolumeText != null) 
            musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void OnSFXVolumeChanged(float value)
    {
        SetMixerVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        if (sfxVolumeText != null) 
            sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    /// <summary>
    /// Converts linear slider value (0-1) to logarithmic decibel value for Audio Mixer.
    /// </summary>
    private void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("Audio Mixer is not assigned in SettingsManager!");
            return;
        }

        // Convert 0-1 slider value to decibels (-80 to 0 dB)
        // Using logarithmic scale for natural volume perception
        float dB = value > 0 ? 20f * Mathf.Log10(value) : -80f;
        audioMixer.SetFloat(parameterName, dB);
    }

    // === GRAPHICS CALLBACKS ===
    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(QUALITY_KEY, index);
    }

    private void OnVSyncChanged(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt(VSYNC_KEY, enabled ? 1 : 0);
    }

    private void OnFullscreenChanged(bool enabled)
    {
        Screen.fullScreen = enabled;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, enabled ? 1 : 0);
    }

    // === GAMEPLAY CALLBACKS ===
    private void OnSensitivityChanged(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
        if (sensitivityText != null) 
            sensitivityText.text = value.ToString("F1");
    }

    private void OnInvertYChanged(bool enabled)
    {
        InvertY = enabled;
        PlayerPrefs.SetInt(INVERT_Y_KEY, enabled ? 1 : 0);
    }

    // === DEVELOPER CALLBACKS ===
    private void OnCheatsChanged(bool enabled)
    {
        CheatsEnabled = enabled;
        PlayerPrefs.SetInt(CHEATS_KEY, enabled ? 1 : 0);
        Debug.Log($"Cheats {(enabled ? "ENABLED" : "DISABLED")}");
    }

    // === PUBLIC METHODS ===
    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
        Debug.Log("Settings reset to defaults");
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings saved successfully");
    }

    private void UpdateAllTexts()
    {
        if (masterVolumeText != null && masterVolumeSlider != null)
            masterVolumeText.text = Mathf.RoundToInt(masterVolumeSlider.value * 100) + "%";
        if (musicVolumeText != null && musicVolumeSlider != null)
            musicVolumeText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100) + "%";
        if (sfxVolumeText != null && sfxVolumeSlider != null)
            sfxVolumeText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100) + "%";
        if (sensitivityText != null && mouseSensitivitySlider != null)
            sensitivityText.text = mouseSensitivitySlider.value.ToString("F1");
    }

    private void OnDestroy()
    {
        // Save on destroy to preserve settings
        PlayerPrefs.Save();
    }
}