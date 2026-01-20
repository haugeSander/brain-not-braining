using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates a simple particle trail that follows the mouse cursor
/// </summary>
public class MouseTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] private GameObject trailDotPrefab; // Will create this if null
    [SerializeField] private int maxTrailDots = 10;
    [SerializeField] private float spawnInterval = 0.03f; // Spawn dot every 0.03s (denser trail)
    [SerializeField] private float dotLifetime = 0.4f;
    [SerializeField] private float minDotSize = 4f;
    [SerializeField] private float maxDotSize = 10f;
    [SerializeField] private Gradient dotColorGradient; // Color gradient over lifetime
    [SerializeField] private bool useGradient = true;

    private float spawnTimer = 0f;
    private Canvas canvas;
    private Sprite circleSprite;

    private void Start()
    {
        // Find canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        // Create circular sprite
        circleSprite = CreateCircleSprite(64);

        // Initialize default gradient if not set
        if (dotColorGradient == null || dotColorGradient.colorKeys.Length == 0)
        {
            dotColorGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(new Color(0, 1, 1), 0f); // Cyan at start
            colorKeys[1] = new GradientColorKey(new Color(0.5f, 0.5f, 1f), 0.5f); // Purple in middle
            colorKeys[2] = new GradientColorKey(new Color(1, 1, 1), 1f); // White at end

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(0.8f, 0f); // Start more opaque
            alphaKeys[1] = new GradientAlphaKey(0f, 1f); // Fade to transparent

            dotColorGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    private Sprite CreateCircleSprite(int resolution)
    {
        // Create a circular texture
        Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[resolution * resolution];

        float center = resolution / 2f;
        float radius = center - 1;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                // Create soft circle with antialiasing
                float alpha = 1f - Mathf.Clamp01((distance - radius) / 2f);
                pixels[y * resolution + x] = new Color(1, 1, 1, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Convert to sprite
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnTrailDot();
        }
    }

    private void SpawnTrailDot()
    {
        if (canvas == null) return;

        // Create trail dot
        GameObject dot = new GameObject("TrailDot");
        dot.transform.SetParent(canvas.transform, false);

        // Add image component
        Image image = dot.AddComponent<Image>();
        image.sprite = circleSprite; // Apply circular sprite
        image.raycastTarget = false; // IMPORTANT: Don't block clicks!

        // Random size for variety
        float randomSize = Random.Range(minDotSize, maxDotSize);

        // Set position to mouse
        RectTransform rectTransform = dot.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(randomSize, randomSize);

        // Convert mouse position to canvas space
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out mousePos);

        rectTransform.anchoredPosition = mousePos;

        // Add fade-out script
        TrailDotFade fade = dot.AddComponent<TrailDotFade>();
        fade.Initialize(dotLifetime, useGradient ? dotColorGradient : null);
    }
}

/// <summary>
/// Helper script to fade out and destroy trail dots
/// </summary>
public class TrailDotFade : MonoBehaviour
{
    private float lifetime;
    private float elapsed;
    private Image image;
    private Vector3 startScale;
    private Gradient colorGradient;
    private bool useGradient;

    public void Initialize(float life, Gradient gradient = null)
    {
        lifetime = life;
        elapsed = 0f;
        image = GetComponent<Image>();
        startScale = transform.localScale;
        colorGradient = gradient;
        useGradient = gradient != null;

        // Set initial color
        if (useGradient)
        {
            image.color = colorGradient.Evaluate(0f);
        }
        else
        {
            image.color = new Color(1, 1, 1, 0.6f); // Default white semi-transparent
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = elapsed / lifetime;

        if (image != null)
        {
            // Use gradient if available, otherwise simple fade
            if (useGradient)
            {
                image.color = colorGradient.Evaluate(progress);
            }
            else
            {
                Color c = image.color;
                c.a = 0.6f * (1f - progress);
                image.color = c;
            }

            // Smooth scale shrink
            float scaleProgress = 1f - (progress * 0.3f); // Shrink to 70% of original
            transform.localScale = startScale * scaleProgress;
        }

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
