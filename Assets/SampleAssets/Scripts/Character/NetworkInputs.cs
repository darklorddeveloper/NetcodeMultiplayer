using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using Unity.Netcode;

namespace StarterAssets
{
    public class NetworkInputs : NetworkBehaviour, IThirdPersonCharacterInput
    {
        public Vector3 move { get; set; }
        public Vector2 look { get; set; }
        public bool jump { get; set; }
        public bool sprint { get; set; }
        private Vector2 originalInput = Vector2.zero;
        private NetworkVariable<Vector3> networkMove = new NetworkVariable<Vector3>();
        private NetworkVariable<Vector2> networkLook = new NetworkVariable<Vector2>();
        private NetworkVariable<bool> networkSprint = new NetworkVariable<bool>();

        public bool analogMovement { get { return _analogMovement; } }

        [SerializeField] private bool _analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private bool hasNewInputToUpdate = false;
        private GameObject mainCamera;

        public void Awake()
        {
            mainCamera = Camera.main.gameObject;
        }

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            hasNewInputToUpdate = true;
            originalInput = value.Get<Vector2>();
            RecalculateMoveInputDirection();
        }

        public void OnLook(InputValue value)
        {
            hasNewInputToUpdate = true;
            look = value.Get<Vector2>();
            RecalculateMoveInputDirection();
        }

        public void OnJump(InputValue value)
        {
            OnJumpServerRPC(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
#endif

        public void MoveInput(Vector3 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            hasNewInputToUpdate = true;
            sprint = newSprintState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void Update()
        {
            if (IsOwner == false)
            {
                UpdateInputFromNetworkVariables();
            }

            if (IsClient && IsOwner)
            {
                UpdateClientInput();
            }
        }

        private void UpdateClientInput()
        {
            if (hasNewInputToUpdate == false)
            {
                return;
            }
            hasNewInputToUpdate = false;
            UpdateInputServerRPC(move, look, sprint);
        }

        private void UpdateInputFromNetworkVariables()
        {
            MoveInput(networkMove.Value);
            LookInput(networkLook.Value);
            SprintInput(networkSprint.Value);
        }

        [ServerRpc]
        public void UpdateInputServerRPC(Vector3 movement, Vector2 lookInput, bool sprintInput)
        {
            networkMove.Value = movement;
            networkLook.Value = lookInput;
            networkSprint.Value = sprintInput;
        }

        [ServerRpc]
        public void OnJumpServerRPC(bool isPressed)
        {
            OnJumpClientRPC(isPressed);
        }

        [ClientRpc]
        public void OnJumpClientRPC(bool isPressed)
        {
            JumpInput(isPressed);
        }

        private void RecalculateMoveInputDirection()
        {
            Vector3 targetDirection = mainCamera.transform.forward * originalInput.y + mainCamera.transform.right * originalInput.x;
            targetDirection.y = 0;
            move = targetDirection.normalized;
        }
    }

}