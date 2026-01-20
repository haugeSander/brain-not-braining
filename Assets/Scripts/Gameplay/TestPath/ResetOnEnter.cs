using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload scenes

public class ResetOnEnter : MonoBehaviour
{
    // Tag of the object that triggers the reset (e.g., "Player")
    public string triggerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering has the correct tag
        if (other.CompareTag(triggerTag))
        {
            // Reload the current active scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}
