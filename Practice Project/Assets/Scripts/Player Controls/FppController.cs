using System;
using System.Security;
using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using Random = System.Random;

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

        [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Rotation Speed of Player")] [SerializeField]
        private float rotationSpeed = 1.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

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

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

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

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        #region Private Variable

        private bool _hasAnimator;
        private const float _threshold = 0.01f;

        //cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        //player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

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
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundCheck()
        {
            //set sphere positon, with offset 
            Vector3 spherePositon = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            //to check the grouned is ture or not 
            Grounded = Physics.CheckSphere(spherePositon, groundedRadius, groundLayer, QueryTriggerInteraction.Ignore);

            //update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            //if there is an input and camera positon is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.DeltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            //clamp out rotation so our values are limited 360 degrees
            _cinemachineTargetYaw = CLampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = CLampAngle(_cinemachineTargetPitch, bottomCameraClamp, topCameraClamp);

            //cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            //float target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

            //a simplistic acceleration and decleration design to be easy to remove, replace, or iterate upon

            //note:  Vector2's == operator uses approximation so is not float point error prone, and is cheaper then magnitude
            //if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            //a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            //analog movement is use to how far the stick is pressed.gradual movement 
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            //accelaration  or decleration to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                //create curved result rather then a linear one giving a more organic speed change
                //note T in lerp in clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                //round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            // if current horizontal speed is == targetspeed
            else
            {
                _speed = targetSpeed;
            }

            //this is to change the speed and blend rate of the animation according to the player speed
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            //normalize input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            //note: vector2's != operator uses approximation so is not floating point error prone, and is cheaper then magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotaion = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                //rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotaion, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            //move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            //update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                //reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;

                //update animator if using character
                //if character is grounded then making the turning off the jump and fall animation
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                //stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                //Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    //the square root of H * -2 * 6 = how much velocity needed to reach desire height
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityRate);

                    //update jump animation for character if using jump is true
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                //jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                //reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                //fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    //update fall animation for character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                //if we are not grounded, do not jump
                _input.jump = false;
            }

            //apply gravity over time if under terminal (Multiply by delta time twice liner to linearaly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravityRate * Time.deltaTime; //delta is to make the gravityrate frame independent and time dependent
            }
        }

        private static float CLampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
            
            //when selected, draw a gizmos in the position of, and matching radius of the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),groundedRadius);
        }

        private void OnFootStep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                //check the length of the footstep audio clip if it is > 0s
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index],transform.TransformPoint(_controller.center));
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center),FootstepAudioVolume);
            }
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