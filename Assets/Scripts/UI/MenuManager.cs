
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
        SceneManager.LoadScene("Level_0_Brainstem");
	}

	public void OnClickScores(){
        Debug.Log("You have clicked the scores button!");
	}

	public void OnClickExit(){
		Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
	}

}
