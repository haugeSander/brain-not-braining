using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using BrainNotBraining.Core;

public class ColorChangingHole : MonoBehaviour
{

    [Header("Countdown UI")]
    public TMP_Text countdownText; 

    [Header("Scene Settings")]
    public string sceneToLoad;

    [Header("Color Setup")]
    public BlockColor[] availableColors = { BlockColor.Red, BlockColor.Green, BlockColor.Yellow, BlockColor.Blue };
    public BlockColor[] requiredColors;
    private int currentIndex = 0;

    [Header("Visuals")]
    public Renderer[] holeRenderers;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    public Color yellowColor = Color.yellow;
    public Color blueColor = Color.blue;

    [Header("UI")]
    public GameObject restartButton;
    public TMP_Text loseText;
    public TMP_Text winText;

    private bool isActive = true;

    void Start()
    {
        GenerateRandomColorOrder();

        currentIndex = 0;
        isActive = true;

        UpdateHoleColor();

        if (loseText != null) loseText.gameObject.SetActive(false);
        if (winText != null) winText.gameObject.SetActive(false);
        if (restartButton != null) restartButton.SetActive(false);
    }

    private void GenerateRandomColorOrder()
    {
        List<BlockColor> temp = new List<BlockColor>();

        foreach (var color in availableColors)
        {
            temp.Add(color);
            temp.Add(color);
        }

        for (int i = 0; i < temp.Count; i++)
        {
            int rand = Random.Range(i, temp.Count);
            (temp[i], temp[rand]) = (temp[rand], temp[i]);
        }

        requiredColors = temp.ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        PushBlock block = other.GetComponent<PushBlock>();
        if (block == null) return;

        if (block.blockColor != requiredColors[currentIndex])
        {
            LoseGame();
            return;
        }

        AcceptBlock(block);
    }

    private void AcceptBlock(PushBlock block)
    {
        block.Consume();
        currentIndex++;

        if (currentIndex >= requiredColors.Length)
        {
            PuzzleCompleted();
        }
        else
        {
            UpdateHoleColor();
        }
    }

    private void PuzzleCompleted()
{
    isActive = false;

    Time.timeScale = 1f;

    if (winText != null)
        winText.gameObject.SetActive(true);

    if (restartButton != null)
        restartButton.SetActive(true);

    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    ProgressionManager.PendingUnlock = BrainRegion.Prefrontal;

    StartCoroutine(LoadNextSceneAfterDelay(5f));
}


    private System.Collections.IEnumerator LoadNextSceneAfterDelay(float delay)
{
    if (countdownText != null)
    {
        countdownText.gameObject.SetActive(true);
    }

    float remaining = delay;
    while (remaining > 0)
    {
        if (countdownText != null)
            countdownText.text = Mathf.Ceil(remaining).ToString();

        yield return null;
        remaining -= Time.unscaledDeltaTime; // Use unscaled time because Time.timeScale = 1
    }

    if (!string.IsNullOrEmpty(sceneToLoad))
        SceneManager.LoadScene(sceneToLoad);
}


    private void LoseGame()
    {
        isActive = false;

        Time.timeScale = 0f;

        if (loseText != null)
            loseText.gameObject.SetActive(true);

        if (restartButton != null)
            restartButton.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateHoleColor()
    {
        Color newColor = GetColorFromEnum(requiredColors[currentIndex]);

        foreach (Renderer r in holeRenderers)
        {
            r.material.color = newColor;
        }
    }

    private Color GetColorFromEnum(BlockColor c)
    {
        switch (c)
        {
            case BlockColor.Red: return redColor;
            case BlockColor.Green: return greenColor;
            case BlockColor.Yellow: return yellowColor;
            case BlockColor.Blue: return blueColor;
        }
        return Color.white;
    }
}
