using UnityEngine;
using UnityEngine.SceneManagement;
using BrainNotBraining.Core;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Detects when the player falls into the void (out of bounds) and resets the level.
    /// Positioned below the playable area as a large trigger collider.
    /// On player trigger enter, reloads the current scene to restart the level.
    /// </summary>
    public class DeathPlane : MonoBehaviour
    {
        [Header("Death Plane Configuration")]
        [Tooltip("Delay before reloading scene (seconds) - allows for death animation/sound")]
        [Range(0f, 10f)]
        public float reloadDelay = 3.0f;

        [Tooltip("Play audio on death (optional)")]
        public AudioClip deathSound;

        [Tooltip("Volume for death sound")]
        [Range(0f, 1f)]
        public float deathSoundVolume = 0.7f;

        [Tooltip("Enable debug logging")]
        public bool debugMode = false;

        // Internal state
        private bool isReloading = false;

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
            // Prevent multiple reload triggers
            if (isReloading)
            {
                return;
            }

            // Check if player fell into void
            if (other.CompareTag("Player"))
            {
                if (debugMode)
                {
                    Debug.Log($"DeathPlane: Player fell into void at position {other.transform.position}");
                }

                isReloading = true;
                OnPlayerDeath();
            }
        }

        /// <summary>
        /// Called when player falls into void. Triggers death sequence.
        /// </summary>
        private void OnPlayerDeath()
        {
            // Play death sound if assigned
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);

                if (debugMode)
                {
                    Debug.Log($"DeathPlane: Playing death sound");
                }
            }

            // Reload level after delay
            if (reloadDelay > 0f)
            {
                Invoke(nameof(ReloadLevel), reloadDelay);
            }
            else
            {
                ReloadLevel();
            }
        }

        /// <summary>
        /// Reloads the current scene to restart the level.
        /// </summary>
        private void ReloadLevel()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (debugMode)
            {
                Debug.Log($"DeathPlane: Reloading scene '{currentSceneName}'");
            }

            // Option 1: Use GameManager if available (preserves progression state)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadLevel(currentSceneName);
            }
            else
            {
                // Option 2: Direct scene reload (fallback)
                SceneManager.LoadScene(currentSceneName);
            }
        }

        /// <summary>
        /// Manual reset method for testing in editor.
        /// </summary>
        [ContextMenu("Test Death Plane")]
        public void TestDeathPlane()
        {
            Debug.Log("DeathPlane: Testing death plane trigger (manual)");
            OnPlayerDeath();
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

            // Draw label
            UnityEditor.Handles.Label(transform.position, "DEATH PLANE");
        }
#endif
    }
}
