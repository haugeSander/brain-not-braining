using UnityEngine;

/// <summary>
/// Manages the overall state and objectives for Level 4.
/// Tracks pickup collection and checks for the win condition.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Game Objective")]
    [Tooltip("The total number of pickups that must be collected to win.")]
    public int totalPickupsInLevel = 0;

    // Internal state
    private int pickupsCollected = 0;

    // Reference to the UI Manager
    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        
        // Automatically find all pickups in the scene to set the total
        totalPickupsInLevel = FindObjectsOfType<SightPickup>().Length;
        
        if (uiManager != null)
        {
            uiManager.UpdatePickupCount(pickupsCollected, totalPickupsInLevel);
        }
    }

    /// <summary>
    /// Called by a pickup when it is collected.
    /// </summary>
    public void OnPickupCollected()
    {
        pickupsCollected++;
        Debug.Log($"Pickup collected! Progress: {pickupsCollected}/{totalPickupsInLevel}");

        if (uiManager != null)
        {
            uiManager.UpdatePickupCount(pickupsCollected, totalPickupsInLevel);
        }

        // Check for win condition
        if (pickupsCollected >= totalPickupsInLevel)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("YOU WIN! All pickups collected.");
        // You can add logic here to show a win screen, load the next level, etc.
        // For now, we'll just freeze the game.
        Time.timeScale = 0; 
    }
}
