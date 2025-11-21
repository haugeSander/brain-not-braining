using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineSceneSwitcher : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "Level_0_Brainstem";

    void Start()
    {
        director.stopped += OnTimelineFinished;
    }

    void OnTimelineFinished(PlayableDirector obj)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
