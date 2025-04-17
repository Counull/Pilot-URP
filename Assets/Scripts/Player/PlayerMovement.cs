using System;
using Data;
using UnityEngine;

namespace Player {
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour {
        [Header("Movement")] [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMult = 2f;


        [Header("Jump")] [SerializeField] private float jumpForce = 5f;
        [SerializeField] private int maxJumpCount = 3;
        [SerializeField] private float jumpInterval = 0.1f;

        [Header("View")] [SerializeField] private Camera eyesCamera;
        [Range(0, 1)] [SerializeField] private float rotateSpeed = 0.5f;
        [SerializeField] private FloatRange viewXRange = new(-80, 80);

        [Header("Ground Check")] [SerializeField]
        private float groundCheckDistance = 0.1f;


        [SerializeField] private LayerMask groundCheckLayers;

        private CharacterController _controller;
        private PlayerInput _playerInput;

        private Vector3 _movementVelocity;
        private Vector3 _groundNormal;

        private float _currentRotationX = 0.0f;

        private bool _isGrounded = false;
        private float _lastJumpTime = 0.0f;
        private int _currentJumpCount;

        public event Action<bool> OnGroundedStatusChange;

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

        #region Movement

        private void LookRotate() {
            var lookInput = _playerInput.Look;
            transform.Rotate(lookInput.x * rotateSpeed * Vector3.up);
            var lookInputY = lookInput.y * rotateSpeed;
            _currentRotationX -= lookInputY;
            _currentRotationX = Mathf.Clamp(_currentRotationX, viewXRange.Min, viewXRange.Max);
            eyesCamera.transform.localEulerAngles = new Vector3(_currentRotationX, 0.0f, 0.0f);
        }


        private void Movement() {
            var moveInput = _playerInput.Movement;
            var newMovement = transform.TransformVector(new Vector3(moveInput.x, 0, moveInput.y)) * moveSpeed;
          

            if (_isGrounded) {
                GroundMovement(moveInput, ref newMovement);
            }
            else {
                AirMovement(moveInput, ref newMovement);
            }

            var canJump = _playerInput.Jump && _currentJumpCount < maxJumpCount &&
                          Time.time - _lastJumpTime > jumpInterval;
            if (canJump) {
                newMovement.y = jumpForce;
                _lastJumpTime = Time.time;
                _isGrounded = false;
                _currentJumpCount++;
            }

            _playerInput.JumpExecuteComplete();

            _movementVelocity = newMovement;
            _controller.Move(_movementVelocity * Time.deltaTime);
        }

        private void GroundMovement(Vector2 input, ref Vector3 newMovement) {
            //Jump
        }

        private void AirMovement(Vector2 input, ref Vector3 newMovement) {
            newMovement.y = _movementVelocity.y;
            newMovement.y += Physics.gravity.y * Time.deltaTime;
        }

        #endregion

        private void GroundCheck() {
            var groundedBefore = _isGrounded;
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
            //检测奇怪角度
            if (Vector3.Dot(hit.normal, transform.up) > 0 &&
                Vector3.Angle(transform.up, hit.normal) <= _controller.slopeLimit) {
                if (hit.distance > _controller.skinWidth) {
                    _controller.Move(Vector3.down * hit.distance);
                }

                _isGrounded = true;
            }

            if (_isGrounded != groundedBefore) {
                GroundedStatusChangeInternal();
                OnGroundedStatusChange?.Invoke(_isGrounded);
            }
        }


        /// <summary>
        /// 当<see cref="GroundCheck"/>检测到状态变化的时候调用
        /// ***并非每一次为<see cref="_isGrounded"/>赋值时被调用***
        /// </summary>
        private void GroundedStatusChangeInternal() {
            if (_isGrounded) {
                _currentJumpCount = 0;
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