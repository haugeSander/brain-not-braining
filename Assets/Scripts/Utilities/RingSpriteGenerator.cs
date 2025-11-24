using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to generate a ring (donut) sprite for timer ring visuals.
/// Attach this to an Image component and it will create a ring sprite on Start.
/// </summary>
public class RingSpriteGenerator : MonoBehaviour
{
    [Header("Ring Settings")]
    [SerializeField] private int resolution = 128; // Texture size
    [SerializeField] private float outerRadius = 0.5f; // Outer edge (0-0.5)
    [SerializeField] private float innerRadius = 0.4f; // Inner edge (creates the hole)
    [SerializeField] private float edgeSoftness = 2f; // Antialiasing amount

    private void Start()
    {
        Image image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("RingSpriteGenerator requires an Image component!");
            return;
        }

        // Generate ring sprite
        Sprite ringSprite = CreateRingSprite(resolution, outerRadius, innerRadius, edgeSoftness);
        image.sprite = ringSprite;

        Debug.Log("Ring sprite generated and applied to Image component");
    }

    private Sprite CreateRingSprite(int res, float outerR, float innerR, float softness)
    {
        Texture2D texture = new Texture2D(res, res, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[res * res];

        float center = res / 2f;
        float outerRadius = center * outerR * 2f; // Scale to texture size
        float innerRadius = center * innerR * 2f;

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                float alpha = 0f;

                // Create ring shape
                if (distance <= outerRadius && distance >= innerRadius)
                {
                    // Inside the ring
                    alpha = 1f;

                    // Soft outer edge
                    if (distance > outerRadius - softness)
                    {
                        alpha = 1f - Mathf.Clamp01((distance - (outerRadius - softness)) / softness);
                    }

                    // Soft inner edge
                    if (distance < innerRadius + softness)
                    {
                        alpha = Mathf.Min(alpha, 1f - Mathf.Clamp01(((innerRadius + softness) - distance) / softness));
                    }
                }

                pixels[y * res + x] = new Color(1, 1, 1, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f));
    }
}
