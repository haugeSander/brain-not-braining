using UnityEngine;
using UnityEngine.InputSystem;
using BrainNotBraining.Core;

namespace BrainNotBraining.Player
{
    /// <summary>
    /// Controls player movement and actions.
    /// Abilities are progressively enabled by ProgressionManager.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // === MOVEMENT SETTINGS ===
        [Header("Movement")]
        [Tooltip("How fast the player moves (enabled in Level 1+)")]
        [SerializeField] private float moveSpeed = 5.0f;

        // === COMPONENTS ===
        private CharacterController characterController;
        private PlayerInputActions inputActions;

        // === INPUT STATE ===
        private Vector2 moveInput;

        // === UNITY LIFECYCLE ===

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
        }

        private void OnDisable()
        {
            inputActions.Disable();
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;
        }

        private void Update()
        {
            // Only move if ProgressionManager says player can move (Motor Cortex unlocked)
            if (!ProgressionManager.Instance.CanPlayerMove()) return;

            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }

        // === INPUT HANDLERS ===

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}
