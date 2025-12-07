using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineSceneSwitcher : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "Level_0_Brainstem";

    [Header("Debug")]
    [SerializeField] private bool enableDebugSkip = true;
    [SerializeField] private KeyCode debugSkipKey = KeyCode.S;

    void Start()
    {
        director.stopped += OnTimelineFinished;
    }

    void Update()
    {
        // Debug: Skip cutscene with S key
        if (enableDebugSkip && Input.GetKeyDown(debugSkipKey))
        {
            Debug.Log("DEBUG: Skipping cutscene, loading next scene");
            SkipCutscene();
        }
    }

    void OnTimelineFinished(PlayableDirector obj)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// Skips the cutscene and loads the next scene immediately
    /// </summary>
    public void SkipCutscene()
    {
        if (director != null && director.state == PlayState.Playing)
        {
            director.Stop();
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
