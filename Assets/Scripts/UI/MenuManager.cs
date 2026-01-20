using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using BrainNotBraining.Core;

public class MenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelSelectManager levelSelectManager;

    void Start()
    {
        Debug.Log("Menu Manager started.");
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
            OnClickPlay();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnClickExit();
    }

    /// <summary>
    /// Opens the level selection panel.
    /// Connect this to your "Play From" button.
    /// </summary>
    public void OnClickPlayFrom()
    {
        if (levelSelectManager != null)
        {
            levelSelectManager.OpenLevelSelectPanel();
        }
        else
        {
            Debug.LogError("LevelSelectManager reference not assigned in MenuManager!");
        }
    }

    /// <summary>
    /// Starts a new game from the beginning.
    /// </summary>
    public void OnClickPlay()
    {
        Debug.Log("You have clicked the play button!");
        SceneManager.LoadScene("IntroCutscene");
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void OnClickExit()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}