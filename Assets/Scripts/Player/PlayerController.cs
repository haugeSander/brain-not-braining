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
            // TODO: Get components
            // characterController = GetComponent<CharacterController>();
            // inputActions = new PlayerInputActions();
            characterController = GetComponent<CharacterController>();
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            // TODO: Enable input actions
            // inputActions.Enable();
            inputActions.Enable();

            // TODO: Subscribe to Move input
            // inputActions.Player.Move.performed += OnMove;
            // inputActions.Player.Move.canceled += OnMove;
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
        }

        private void OnDisable()
        {
            // TODO: Disable and unsubscribe
            inputActions.Disable();
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;

        }

        private void Update()
        {
            // TODO: Handle movement every frame
            // Only if ProgressionManager says player can move!

            // STEPS:
            // 1. Check if player can move:
            //    if (!ProgressionManager.Instance.CanPlayerMove()) return;
            // 2. Calculate movement direction:
            //    Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            // 3. Apply movement:
            //    characterController.Move(move * moveSpeed * Time.deltaTime);

            if (!ProgressionManager.Instance.CanPlayerMove()) return;
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }

        // === INPUT HANDLERS ===

        private void OnMove(InputAction.CallbackContext context)
        {
            // TODO: Read movement input
            // moveInput = context.ReadValue<Vector2>();
            // This captures WASD input as a 2D vector

            moveInput = context.ReadValue<Vector2>();
        }
    }
}
