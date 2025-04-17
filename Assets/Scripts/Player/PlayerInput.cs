using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerInput : MonoBehaviour {
        public delegate void CursorLockStatusChange(bool isLocked);

        public event CursorLockStatusChange OnCursorLockChanged;


        private InputAction _movement;
        private InputAction _look;
        private InputAction _jump;


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
            _jump.performed += ctx => Jump = true;
        }

        public void JumpExecuteComplete() {
            Jump = false;
        }


        private void Start() {
            LockCursor = true;
        }


        void ActivePlayerInput(bool active) {
            if (active) {
                _movement.Enable();
                _look.Enable();
                _jump.Enable();
                return;
            }

            _movement.Disable();
            _look.Disable();
            _jump.Disable();
        }
    }
}