using Data;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour {
        [Header("Movement")] [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask groundCheckLayers;

        [Header("View")] [SerializeField] private Camera eyesCamera;
        [Range(0, 1)] [SerializeField] private float rotateSpeed = 0.5f;
        [SerializeField] private FloatRange viewXRange = new(-80, 80);


        private CharacterController _controller;
        private PlayerInput _playerInput;
        private float _currentRotationX = 0.0f;

        private Vector3 _movementVelocity;
        private Vector3 _groundNormal;
        private bool _isGrounded = false;


        private float _lastJumpTime = 0.0f;

        void Awake() {
            //Component rigidbody is deprecated
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();

            if (eyesCamera == null) {
                eyesCamera = Camera.main;
            }
        }


        void Update() {
            GroundCheck();
            LookRotate();
            Movement();
        }


        private void Movement() {
            var moveInput = _playerInput.Movement;
            var newMovement = transform.TransformVector(new Vector3(moveInput.x, 0, moveInput.y)) * moveSpeed;


            if (_isGrounded) {
                if (_playerInput.Jump) {
                    newMovement.y = jumpForce;
                    _lastJumpTime = Time.time;
                    _isGrounded = false;
                }
            }
            else {
                newMovement.y = _movementVelocity.y;
                newMovement.y += Physics.gravity.y * Time.deltaTime;
            }

            _movementVelocity = newMovement;
            _controller.Move(_movementVelocity * Time.deltaTime);
        }

        private void LookRotate() {
            var lookInput = _playerInput.Look;
            transform.Rotate(lookInput.x * rotateSpeed * Vector3.up);
            var lookInputY = lookInput.y * rotateSpeed;
            _currentRotationX -= lookInputY;
            _currentRotationX = Mathf.Clamp(_currentRotationX, viewXRange.Min, viewXRange.Max);
            eyesCamera.transform.localEulerAngles = new Vector3(_currentRotationX, 0.0f, 0.0f);
        }

        private void GroundCheck() {
            _isGrounded = false;
            _groundNormal = Vector3.up;
            if (Time.time < _lastJumpTime + 0.1f) return;


            if (!Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(),
                    _controller.radius, Vector3.down, out RaycastHit hit,
                    _controller.skinWidth + groundCheckDistance, groundCheckLayers,
                    QueryTriggerInteraction.Ignore)) {
                return;
            }

            _groundNormal = hit.normal;
            if (Vector3.Dot(hit.normal, transform.up) > 0 &&
                Vector3.Angle(transform.up, hit.normal) <= _controller.slopeLimit) {
                _isGrounded = true;
                if (hit.distance > _controller.skinWidth) {
                    _controller.Move(Vector3.down * hit.distance);
                }
            }
        }


        private void OnDrawGizmos() { }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere() {
            return transform.position + (transform.up * _controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere() {
            return transform.position + (transform.up * (_controller.height - _controller.radius));
        }
    }
}