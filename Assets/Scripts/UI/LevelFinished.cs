
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelFinished : MonoBehaviour
{

	void Start ()
    {
        Debug.Log("Level finished started.");
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
            OnMainMenuClick();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnNextLevelClick();
 

        
    }

	public void OnMainMenuClick(){
        Debug.Log("You have clicked the menu button!");
        SceneManager.LoadScene("MainMenu");
	}

    // next level to do
	public void OnNextLevelClick(){
		Debug.Log("You have clicked the next button!");
        SceneManager.LoadScene("MainMenu");
	}

}
