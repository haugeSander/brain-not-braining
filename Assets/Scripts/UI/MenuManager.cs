
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class MenuManager : MonoBehaviour
{

	void Start ()
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

	public void OnClickPlay(){
        Debug.Log("You have clicked the play button!");
        SceneManager.LoadScene("IntroCutscene");
	}


	public void OnClickExit(){
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
	}

}
