using System;
using System.Security;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrototypeProject
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FppController : MonoBehaviour
    {
        #region Reference

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        #endregion

        #region Variables/Tools

        [Header("Player")] [Tooltip("Movement Speed of Player in m/s")] [SerializeField]
        private float moveSpeed = 4.0f;

        [Tooltip("Sprint Speed of Player in m/s")] [SerializeField]
        private float sprintSpeed = 6.0f;

        [Tooltip("Rotation Speed of Player")] [SerializeField]
        private float rotationSpeed = 1.0f;

        [Tooltip("Acceleration and Decleration")] [SerializeField]
        private float accelerationRate = 10f;

        [SerializeField] private float decelerationRate = 10f;


        [Space(10)] [Tooltip("The height the player can jump")] [SerializeField]
        private float jumpHeight = 1.2f;

        [Tooltip("Gravity rate for Player (g = 9.8 m/s)")] [SerializeField]
        private float gravityRate = -15.0f;


        [Space(10)] [Tooltip("Time interval for next jump. set to of to instantly jump again")] [SerializeField]
        private float jumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. useful for walking down stairs")]
        [SerializeField]
        private float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("This is the check if the player is on the ground or not. required for jump")]
        [SerializeField]
        private bool isPlayerGrounded = true;

        [Tooltip("Useful for rough Ground")] public float groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. should match the radius of the CharacterController")]
        [SerializeField]
        private float groundedRadius = 0.5f;

        [Tooltip("What layer the character uses as ground checks")] [SerializeField]
        private LayerMask groundLayer;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the cinemachine Virtual camera that will follow player")]
        [SerializeField]
        private GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")] [SerializeField]
        private float topCameraClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        private float bottomCameraClamp = -90.0f;

        #region Private Variable

        private bool _hasAnimator;
        private const float _threshold = 0.01f;

        //cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        //player
        private float _speed;

        //timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        //animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        #endregion

        #endregion

        #region Struct

        #endregion

        #region Functions

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void AssignAnimationIDs()
        {
        }

        private void GroundCheck()
        {
        }

        private void CameraRotation()
        {
        }

        private void Move()
        {
        }

        private void JumpAndGravity()
        {
        }

        // private static float CLampAngle(float lfAngle, float lfMin, float lfMax)
        // {
        //     
        // }

        private void OnDrawGizmosSelected()
        {
        }

        private void OnFootStep(AnimationEvent animationEvent)
        {
        }

        private void OnLand(AnimationEvent animationEvent)
        {
        }

        #endregion

        #region Awake/LateUpdate

        private void Awake()
        {
            //get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        #endregion


        #region Start/Update

        void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            AssignAnimationIDs();

            // reset out timeout on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }


        void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            JumpAndGravity();
            GroundCheck();
            Move();
        }

        #endregion
    }
}