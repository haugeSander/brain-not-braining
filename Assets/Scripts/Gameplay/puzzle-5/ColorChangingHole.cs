using UnityEngine;

public class ColorChangingHole : MonoBehaviour
{
    [Header("Color Sequence")]
    public BlockColor[] requiredColors;
    private int currentIndex = 0;

    [Header("Visuals")]
    public Renderer[] holeRenderers; // MULTIPLE renderers
    public Color[] displayColors;

    private bool isActive = true;

    void Start()
    {
        UpdateHoleColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        PushBlock block = other.GetComponent<PushBlock>();
        if (block == null) return;

        if (block.blockColor != requiredColors[currentIndex]) return;

        AcceptBlock(block);
    }

    private void AcceptBlock(PushBlock block)
    {
        block.Consume();
        currentIndex++;

        if (currentIndex >= requiredColors.Length)
        {
            isActive = false;
            Debug.Log("Puzzle completed!");
        }
        else
        {
            UpdateHoleColor();
        }
    }

    private void UpdateHoleColor()
    {
        Color newColor = displayColors[currentIndex];

        foreach (Renderer r in holeRenderers)
        {
            r.material.color = newColor;
        }
    }
}