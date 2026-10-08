using System;
using UnityEngine.InputSystem;
using System.IO.Enumeration;
using UnityEngine;

namespace PrototypeProject
{
    public class ObjectGrabbable : MonoBehaviour
    {
        #region References

#if ENABLE_INPUT_SYSTEM
        [SerializeField] private PlayerInput _playerInput;
#endif
        private Rigidbody _rb;
        public Transform objectGrabPointTransform;
        [SerializeField] private Transform objectGrabPointTransformOnFocus;
        [SerializeField] private StarterAssetsInputs _inputs;
        [SerializeField] private BasicRigidBodyPush _playerRigidBodyPush;

        #endregion

        #region Variables

        [SerializeField] private float mouseSensitivity = 0.5f;
        [SerializeField] private float controllerSensitivity = 90.0f;
        private float lerpGrab = 15.0f;
        [SerializeField] private float TargetYaw;
        [SerializeField] private float TargetPitch;
        private float RotationSpeedYaw = 1f;
        private float RotationSpeedPitch = 1f;
        [SerializeField] private float _threshold = 0.01f;

        #endregion

        #region Functions

        /// <summary>
        /// This is a Grab function for the player to Grab object when the "E" key is pressed.
        /// Rigid body Kinematic is on.
        /// player rigid body push off.
        /// gravity is off.
        /// Storing object grab point transform position into the grabbable object.
        /// </summary>
        /// <param name="objectGrabPointTransform"></param>
        public void Grab(Transform objectGrabPointTransform)
        {
            // _rb.isKinematic = true;
            _rb.detectCollisions = false;
            _playerRigidBodyPush.canPush = false;
            _rb.useGravity = false;
            this.objectGrabPointTransform = objectGrabPointTransform;
        }

        /// <summary>
        /// This is a drop function for the player to drop object when the "E" key is pressed.
        /// Rigid body Kinematic is off.
        /// player rigid body on.
        /// gravity is on.
        /// restoring grabbable object to it's current position.
        /// </summary>
        public void Drop()
        {
            this.objectGrabPointTransform = null;
            _rb.detectCollisions = true;
            _rb.useGravity = true;
            _playerRigidBodyPush.canPush = true;
            // _rb.isKinematic = false;
        }

        public void RotateGrabbableObjectOnFocus()
        {
            if (objectGrabPointTransform != null && !_inputs.focus)
            {
                if (_inputs.look.sqrMagnitude <= _threshold)
                {
                    TargetYaw = 0.0f;
                    TargetPitch = 0.0f;
                }
            }

            if (objectGrabPointTransform != null && _inputs.focus)
            {
                _rb.freezeRotation = false;
                if (_inputs.look.sqrMagnitude >= _threshold)
                {
                    float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                    TargetYaw += _inputs.look.x * RotationSpeedYaw * deltaTimeMultiplier;
                    TargetPitch += _inputs.look.y * RotationSpeedPitch * deltaTimeMultiplier;
                }

                transform.rotation = Quaternion.Euler(TargetPitch, TargetYaw, 0.0f);
                // transform.Rotate(TargetPitch, TargetYaw, 0f);
                // _rb.rotation = Quaternion.Euler(TargetPitch, TargetYaw, 0.0f);
            }
        }

        public bool IsCurrentDeviceMouse
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

        public void GrabTransformation()
        {
            if (objectGrabPointTransform != null && !_inputs.focus)
            {
                _rb.interpolation = RigidbodyInterpolation.Interpolate;
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position,
                    lerpGrab * Time.deltaTime);
                _rb.MovePosition(targetPosition);
            }

            if (objectGrabPointTransform != null && _inputs.focus)
            {
                _rb.interpolation = RigidbodyInterpolation.None;
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransformOnFocus.position,
                    lerpGrab * Time.deltaTime);
                _rb.MovePosition(targetPosition);
            }
        }

        #endregion

        #region Awake/Fixed Update

        private void FixedUpdate()
        {
           
        }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        #endregion

        #region Start/Update

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.linearDamping = 5.0f;
            _rb.angularDamping = 5.0f;
            _rb.freezeRotation = false;
            _rb.isKinematic = false;
            _rb.detectCollisions = true;
            _inputs = FindFirstObjectByType<StarterAssetsInputs>();
            _playerRigidBodyPush = FindFirstObjectByType<BasicRigidBodyPush>();
            //This is a method to find gameObject transform in runtime so you don't have to manually put things on your own
            GameObject targetTransform = GameObject.FindGameObjectWithTag("OnFocusGrabPoint");
            objectGrabPointTransformOnFocus = targetTransform.transform;
#if ENABLE_INPUT_SYSTEM
            _playerInput = FindFirstObjectByType<PlayerInput>();
#endif
        }

        // Update is called once per frame
        void Update()
        {
            RotateGrabbableObjectOnFocus();
            GrabTransformation();
        }

        #endregion
    }
}