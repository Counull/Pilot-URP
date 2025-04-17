using UnityEngine.InputSystem;

namespace Player {
    public class HoldOrToggleInput {
        private bool _switched = false;
        private bool _active = false;
        public readonly InputAction Action;

        public bool Switched {
            get => _switched;
            private set {
                if (_switched != value) {
                    if (value) {
                        Action.performed += ActiveChange;
                    }
                    else {
                        Action.performed -= ActiveChange;
                    }
                }

                _switched = value;
            }
        }

        private void ActiveChange(InputAction.CallbackContext context) {
            _active = !_active;
        }

        public bool IsActive => _switched ? _active : Action.IsPressed();

        public HoldOrToggleInput(InputAction action, bool switched = false) {
            Action = action;
            Switched = switched;
        }
    }
}