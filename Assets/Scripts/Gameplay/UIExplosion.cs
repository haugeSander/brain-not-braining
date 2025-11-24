using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Simple UI-based explosion effect that works reliably with Canvas
/// Creates expanding colored circles that fade out
/// </summary>
public class UIExplosion : MonoBehaviour
{
    public static void Create(Vector2 position, Color color, Transform parent)
    {
        GameObject explosionObj = new GameObject("UIExplosion");
        explosionObj.transform.SetParent(parent, false);

        // Add RectTransform
        RectTransform rect = explosionObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(50f, 50f);

        // Add Image component
        Image image = explosionObj.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        // Create circular sprite
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[64 * 64];

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dx = x - 32f;
                float dy = y - 32f;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = 1f - Mathf.Clamp01((distance - 30f) / 2f);
                pixels[y * 64 + x] = new Color(1, 1, 1, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;

        // Add animation component
        UIExplosionAnimator animator = explosionObj.AddComponent<UIExplosionAnimator>();
        animator.Initialize(color);
    }
}

public class UIExplosionAnimator : MonoBehaviour
{
    private Image image;
    private RectTransform rect;
    private Color initialColor;
    private float duration = 0.5f;
    private float elapsed = 0f;
    private Vector3 startScale;
    private Vector3 endScale;

    public void Initialize(Color color)
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        initialColor = color;
        startScale = Vector3.one * 0.5f;
        endScale = Vector3.one * 2.5f;

        rect.localScale = startScale;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Expand and fade
        rect.localScale = Vector3.Lerp(startScale, endScale, t);

        Color c = initialColor;
        c.a = 1f - t; // Fade out
        image.color = c;
    }
}
