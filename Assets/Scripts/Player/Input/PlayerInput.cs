using System;
using CustomAttribute;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player {
    public class PlayerInput : MonoBehaviour {
        public delegate void CursorLockStatusChange(bool isLocked);

        public event CursorLockStatusChange OnCursorLockChanged;

        [SerializeField] private bool switchSprint = true;

        [InGameInputAction] private InputAction _movement;
        [InGameInputAction] private InputAction _look;
        [InGameInputAction] private InputAction _jump;
        [InGameInputAction] private InputAction _dash;


        public bool CanProcessInput => LockCursor;
        public Vector2 Movement => _movement.ReadValue<Vector2>();
        public Vector2 Look => _look.ReadValue<Vector2>();


        /// <summary>
        /// Jump triggered in current update,
        ///*** NOT VALID in fixed update ***
        /// </summary>
        public bool JumpTriggeredInFrame => _jump.WasPerformedThisFrame();


        public bool Jump { get; private set; }

        public bool LockCursor {
            get => !Cursor.visible && Cursor.lockState == CursorLockMode.Locked;
            set {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !value;
                ActivePlayerInput(value);
                OnCursorLockChanged?.Invoke(value);
            }
        }

        private void Awake() {
            _movement = InputSystem.actions.FindAction("Move");
            _look = InputSystem.actions.FindAction("Look");
            _jump = InputSystem.actions.FindAction("Jump");
            _dash = InputSystem.actions.FindAction("Dash");
            InGameInputActionAttribute.CatchAllInputAction(this);

            _jump.performed += ctx => Jump = true;
        }


        public void JumpExecuteComplete() {
            Jump = false;
        }


        private void Start() {
            LockCursor = true;
        }


        void ActivePlayerInput(bool active) {
            InGameInputActionAttribute.SetAllActionEnable(active);
        }
    }
}