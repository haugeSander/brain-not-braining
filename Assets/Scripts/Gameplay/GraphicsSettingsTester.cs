using UnityEngine;

/// <summary>
/// Displays current graphics settings on screen for testing.
/// Add to any GameObject and press G to toggle the display.
/// </summary>
public class GraphicsSettingsTester : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.G;
    public bool showOnStart = true;

    private bool showDebug = true;
    private GUIStyle labelStyle;
    private GUIStyle headerStyle;

    private void Start()
    {
        showDebug = showOnStart;

        // Setup GUI styles
        labelStyle = new GUIStyle();
        labelStyle.fontSize = 14;
        labelStyle.normal.textColor = Color.white;
        labelStyle.padding = new RectOffset(5, 5, 2, 2);

        headerStyle = new GUIStyle(labelStyle);
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.yellow;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            showDebug = !showDebug;
        }
    }

    private void OnGUI()
    {
        if (!showDebug) return;

        // Create semi-transparent background
        GUI.Box(new Rect(10, 10, 400, 500), "");

        float y = 20;
        float lineHeight = 22;

        // Header
        GUI.Label(new Rect(20, y, 380, 30), "GRAPHICS SETTINGS TEST", headerStyle);
        y += 35;

        // Quality Settings
        GUI.Label(new Rect(20, y, 380, 20), $"Quality Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"VSync: {GetVSyncStatus()}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Target FPS: {Application.targetFrameRate} (-1 = unlimited)", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Current FPS: {GetFPS()}", labelStyle);
        y += lineHeight;

        // Screen Settings
        y += 10;
        GUI.Label(new Rect(20, y, 380, 20), "SCREEN", headerStyle);
        y += 25;

        GUI.Label(new Rect(20, y, 380, 20), $"Fullscreen: {Screen.fullScreen}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Resolution: {Screen.width}x{Screen.height}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Refresh Rate: {Screen.currentResolution.refreshRate}Hz", labelStyle);
        y += lineHeight;

        // Advanced Quality Settings
        y += 10;
        GUI.Label(new Rect(20, y, 380, 20), "QUALITY DETAILS", headerStyle);
        y += 25;

        GUI.Label(new Rect(20, y, 380, 20), $"Shadow Quality: {QualitySettings.shadows}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Shadow Resolution: {QualitySettings.shadowResolution}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Shadow Distance: {QualitySettings.shadowDistance}m", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Texture Quality: {GetTextureQuality()}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Anisotropic Filtering: {QualitySettings.anisotropicFiltering}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Anti Aliasing: {GetAntiAliasingLevel()}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Soft Particles: {QualitySettings.softParticles}", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Realtime Reflection Probes: {QualitySettings.realtimeReflectionProbes}", labelStyle);
        y += lineHeight;

        // Performance Info
        y += 10;
        GUI.Label(new Rect(20, y, 380, 20), "PERFORMANCE", headerStyle);
        y += 25;

        GUI.Label(new Rect(20, y, 380, 20), $"Used Memory: {GetUsedMemory()} MB", labelStyle);
        y += lineHeight;

        GUI.Label(new Rect(20, y, 380, 20), $"Time Scale: {Time.timeScale}", labelStyle);
        y += lineHeight;

        // Controls hint
        y += 20;
        GUI.Label(new Rect(20, y, 380, 20), $"Press '{toggleKey}' to hide/show this panel", labelStyle);
    }

    private string GetVSyncStatus()
    {
        switch (QualitySettings.vSyncCount)
        {
            case 0: return "OFF";
            case 1: return "Every VBlank";
            case 2: return "Every Second VBlank";
            default: return QualitySettings.vSyncCount.ToString();
        }
    }

    private string GetFPS()
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        string color = fps >= 60 ? "green" : fps >= 30 ? "yellow" : "red";
        return $"<color={color}>{fps:F0}</color>";
    }

    private string GetTextureQuality()
    {
        switch (QualitySettings.globalTextureMipmapLimit)
        {
            case 0: return "Full Res";
            case 1: return "Half Res";
            case 2: return "Quarter Res";
            case 3: return "Eighth Res";
            default: return QualitySettings.globalTextureMipmapLimit.ToString();
        }
    }

    private string GetAntiAliasingLevel()
    {
        int aa = QualitySettings.antiAliasing;
        if (aa == 0) return "OFF";
        return $"{aa}x MSAA";
    }

    private string GetUsedMemory()
    {
        float memoryMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
        return memoryMB.ToString("F1");
    }

    /// <summary>
    /// Test all quality levels and log the results.
    /// </summary>
    [ContextMenu("Test All Quality Levels")]
    public void TestAllQualityLevels()
    {
        Debug.Log("=== Testing All Quality Levels ===");

        int currentLevel = QualitySettings.GetQualityLevel();

        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i);

            Debug.Log($"\n--- Quality Level {i}: {QualitySettings.names[i]} ---");
            Debug.Log($"  Shadows: {QualitySettings.shadows}");
            Debug.Log($"  Shadow Distance: {QualitySettings.shadowDistance}");
            Debug.Log($"  Shadow Resolution: {QualitySettings.shadowResolution}");
            Debug.Log($"  Texture Quality: {GetTextureQuality()}");
            Debug.Log($"  Anti-Aliasing: {GetAntiAliasingLevel()}");
            Debug.Log($"  Anisotropic: {QualitySettings.anisotropicFiltering}");
            Debug.Log($"  Soft Particles: {QualitySettings.softParticles}");
        }

        // Restore original quality level
        QualitySettings.SetQualityLevel(currentLevel);
        Debug.Log($"\n✓ Test complete. Restored to: {QualitySettings.names[currentLevel]}");
    }

    /// <summary>
    /// Test VSync settings.
    /// </summary>
    [ContextMenu("Test VSync")]
    public void TestVSync()
    {
        Debug.Log("=== Testing VSync ===");

        Debug.Log($"Current VSync: {QualitySettings.vSyncCount}");
        Debug.Log($"Current FPS: {1.0f / Time.deltaTime:F1}");
        Debug.Log($"Target FPS: {Application.targetFrameRate}");
        Debug.Log($"Screen Refresh Rate: {Screen.currentResolution.refreshRate}Hz");

        Debug.Log("\nVSync OFF should give higher FPS");
        Debug.Log("VSync ON (1) should cap at refresh rate");
    }
}