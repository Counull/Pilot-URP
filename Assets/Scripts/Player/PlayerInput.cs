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
        public bool Jump => _jump.triggered;

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