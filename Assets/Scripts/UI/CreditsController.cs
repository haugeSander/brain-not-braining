using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class CreditsController : MonoBehaviour
{
    [Header("Audio Mixer Groups")]
    [Tooltip("Assign your mixer groups for proper volume control")]
    public AudioMixerGroup musicMixerGroup;

    [Header("UI")]
    [SerializeField] private GameObject creditsPanel;
    
    [Header("Credits Content")]
    [TextArea(10, 20)]
    [SerializeField]
    private string creditsContent = @"<size=72><b>Brain Not Braining</b></size>

<size=46><i>A Game About Discovery</i></size>
\n\n

<size=40><b>Created By</b></size>
\n
<size=32>Giulio Francesco Zemignani</size>
<size=32>Sander Hauge</size>
<size=32>Christopher Auer</size>
\n\n\n

<size=40><b>Roles</b></size>
\n
<size=30><b>Lead Programmer</b></size>
<size=28>Giulio Francesco Zemignani</size>
\n
<size=30><b>Lead Designer & Programmer</b></size>
<size=28>Sander Hauge</size>
\n
<size=30><b>QA & Programmer</b></size>
<size=28>Christopher Auer</size>
\n\n\n

<size=40><b>Sound & Music</b></size>
\n
<size=28>pixabay</size>
\n\n\n

<size=40><b>Academic Project</b></size>
\n\n
<size=28>This game was developed as part of the</size>
<size=28><i>Entretenimiento y Videojuegos</i> course</size>
<size=28>at Universitat Politècnica de València (UPV),</size>
<size=28>during the Autumn Semester 2025.</size>
\n
<size=28>All team members participated as</size>
<size=28>Erasmus students studying in Valencia.</size>
\n\n\n\n

<size=48><b>Thank You For Playing!</b></size>
\n\n
<size=24><i>The mouse brain remembers…</i></size>";

    [Header("Text Styling")]
    [SerializeField] private Font creditsFont;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Center;
    [SerializeField] private float baseFontSize = 1f;

    [Header("Scrolling")]
    [SerializeField] private float scrollSpeed = 40f;
    [SerializeField] private float finalLineHoldDuration = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip creditsMusic;

    [Header("Navigation")]
    [SerializeField] private string returnScene = "MainMenu";

    private bool isRunning;
    private TextMeshProUGUI creditsText;
    
    private void Awake()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicMixerGroup;
            musicSource.loop = false;
            musicSource.playOnAwake = false;
        }
    }

    public void PlayCredits()
    {
        if (isRunning) return;
        StartCoroutine(RunCredits());
    }

    private IEnumerator RunCredits()
    {
        isRunning = true;

        // 1. Show panel first
        creditsPanel.SetActive(true);

        // 2. Find or create TextMeshProUGUI component
        creditsText = creditsPanel.GetComponentInChildren<TextMeshProUGUI>();
        
        if (creditsText == null)
        {
            // Create a new TextMeshProUGUI if one doesn't exist
            GameObject textObject = new GameObject("CreditsText");
            textObject.transform.SetParent(creditsPanel.transform, false);
            
            creditsText = textObject.AddComponent<TextMeshProUGUI>();
            
            // Setup RectTransform to fill parent
            RectTransform rectTransform = creditsText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        // 3. Configure text component
        creditsText.fontSize = baseFontSize;
        creditsText.enableAutoSizing = false;
        creditsText.alignment = alignment;
        creditsText.color = textColor;
        
        if (creditsFont != null)
        {
            creditsText.font = TMP_FontAsset.CreateFontAsset(creditsFont);
        }

        // 4. Assign text content
        creditsText.text = creditsContent;

        // 5. Force TMP + layout update
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsText.rectTransform);
        creditsText.ForceMeshUpdate();

        // 6. Move text off-screen BEFORE first frame
        Canvas canvas = creditsText.GetComponentInParent<Canvas>();
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        creditsText.rectTransform.anchoredPosition = new Vector2(0, -canvasHeight);

        // 7. Start music
        if (musicSource != null && creditsMusic != null)
        {
            Debug.Log("Playing credits music");
            musicSource.clip = creditsMusic;
            musicSource.Play();
        }
        else
        {
            if (musicSource == null)
                Debug.LogError("Music source is null!");
            if (creditsMusic == null)
                Debug.LogError("Credits music clip is not assigned!");
        }

        // 8. Scroll + center final line
        yield return StartCoroutine(ScrollCreditsAndCenterFinalLine());

        SceneManager.LoadScene(returnScene);
    }

    private IEnumerator ScrollCreditsAndCenterFinalLine()
    {
        RectTransform rect = creditsText.rectTransform;

        creditsText.ForceMeshUpdate();
        TMP_TextInfo info = creditsText.textInfo;

        if (info.lineCount == 0)
        {
            Debug.LogError("CreditsText has no lines. Did you forget to assign the text?");
            yield break;
        }

        TMP_LineInfo lastLine = info.lineInfo[info.lineCount - 1];

        Canvas canvas = creditsText.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;

        float targetY =
            (canvasHeight / 2f)
            - lastLine.ascender
            - lastLine.baseline;

        while (rect.anchoredPosition.y < targetY)
        {
            rect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = new Vector2(0, targetY);
        yield return new WaitForSeconds(finalLineHoldDuration);
    }
}