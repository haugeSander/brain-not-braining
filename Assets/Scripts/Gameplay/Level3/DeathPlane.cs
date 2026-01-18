using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Detects when the player enters a lethal area and triggers the death sequence.
    /// In Level 4, this calls the LevelManager to show the death screen.
    /// In other levels, it can fall back to reloading the scene.
    /// </summary>
    public class DeathPlane : MonoBehaviour
    {
        [Header("Death Plane Configuration")]
        [Tooltip("Play audio on death (optional)")]
        public AudioClip deathSound;

        [Tooltip("Volume for death sound")]
        [Range(0f, 1f)]
        public float deathSoundVolume = 0.7f;

        [Tooltip("Enable debug logging")]
        public bool debugMode = false;

        private bool playerIsDead = false;

        private void Awake()
        {
            // Ensure this GameObject has a trigger collider
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogError("DeathPlane: No Collider component found! Please add a BoxCollider and set isTrigger = true.");
            }
            else if (!col.isTrigger)
            {
                Debug.LogWarning("DeathPlane: Collider isTrigger is false! Setting to true.");
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerIsDead) return;

            if (other.CompareTag("Player"))
            {
                if (debugMode)
                {
                    Debug.Log($"DeathPlane: Player entered death trigger at position {other.transform.position}");
                }

                playerIsDead = true;
                OnPlayerDeath();
            }
        }

        /// <summary>
        /// Called when player enters the death trigger.
        /// </summary>
        private void OnPlayerDeath()
        {
            // Play death sound if assigned
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
            }

            // Use the LevelManager if it exists (for Level 4's death screen)
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.TriggerPlayerDeath();
            }
            else
            {
                // Fallback for other scenes that might not have a LevelManager
                Debug.LogWarning("DeathPlane: LevelManager not found. Reloading scene as a fallback.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw gizmo in editor to visualize death plane area.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Red semi-transparent
            Gizmos.matrix = transform.localToWorldMatrix;

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                if (col is BoxCollider boxCol)
                {
                    Gizmos.DrawCube(boxCol.center, boxCol.size);
                }
                else if (col is SphereCollider sphereCol)
                {
                    Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
                }
            }
        }
#endif
    }
}
