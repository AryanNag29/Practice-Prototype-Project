using System;
using UnityEngine.InputSystem;
using System.IO.Enumeration;
using UnityEngine;

namespace PrototypeProject
{
    public class ObjectGrabbable : MonoBehaviour
    {
        #region References

        [SerializeField] private Camera _maincamera;
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private PlayerInput _playerInput;
#endif
        private Rigidbody _rb;
        public Transform objectGrabPointTransform;
        [SerializeField] private Transform objectGrabPointTransformOnFocus;
        [SerializeField] private StarterAssetsInputs _inputs;
        public BasicRigidBodyPush _playerRigidBodyPush;

        #endregion

        #region Variables

        //Grab Timer
        private float TimeSinceGrab = 0f;
        private bool TimeOverSinceGrab = false;

        //Focus state driven by Camera Event
        private bool _isFocusing = false;

        //Grab sensi
        [SerializeField] private float mouseSensitivity = 0.5f;
        [SerializeField] private float controllerSensitivity = 90.0f;

        //grab lerp (t)
        private float lerpGrab = 10.0f;

        //Grab Rotation
        private Quaternion _currentRotation;
        [SerializeField] private float TargetYaw;
        [SerializeField] private float TargetPitch;
        private float RotationSpeedYaw = 1f;
        private float RotationSpeedPitch = 1f;
        [SerializeField] private float _threshold = 0.01f;

        #endregion

        #region Event Subscription

        private void OnEnable()
        {
            InteractionEvents.OnGrabRequested += HandleGrabRequested;
            InteractionEvents.OnDropRequested += HandleDropRequest;
        }

        private void OnDisable()
        {
            InteractionEvents.OnGrabRequested -= HandleGrabRequested;
            InteractionEvents.OnDropRequested -= HandleDropRequest;
        }

        private void HandleGrabRequested(ObjectGrabbable target, Transform grabPoint)
        {
            //Ignore event if player grabbed a different object
            if (target != this) return;
            Grab(grabPoint);
        }

        private void HandleDropRequest()
        {
            //Ignore event if this object is not currently held
            if (objectGrabPointTransform == null) return;
            Drop();
        }

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
            if (_playerRigidBodyPush != null) _playerRigidBodyPush.canPush = false;
            _rb.useGravity = false;
            this.objectGrabPointTransform = objectGrabPointTransform;

            //Reset timer and set damping ones on Grab
            TimeSinceGrab = 0f;
            TimeOverSinceGrab = false;
            RigidBodyGrabSetting(true);
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
            if (_playerRigidBodyPush != null) _playerRigidBodyPush.canPush = true;
            // _rb.isKinematic = false;

            //Reset timer and restore damping ones on drop
            TimeSinceGrab = 0f;
            TimeOverSinceGrab = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            RigidBodyGrabSetting(false);
        }

        public void RotateGrabbableObjectOnFocus()
        {
            // Use _inputs.focus directly
            if (!_inputs.focus)
            {
                if (_inputs.look.sqrMagnitude <= _threshold)
                {
                    TargetYaw = 0.0f;
                    TargetPitch = 0.0f;
                }

                return;
            }

            _rb.freezeRotation = false;
            if (_inputs.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                TargetYaw += _inputs.look.x * RotationSpeedYaw * deltaTimeMultiplier;
                TargetPitch += _inputs.look.y * RotationSpeedPitch * deltaTimeMultiplier;
            }

            transform.rotation = Quaternion.Euler(TargetPitch, TargetYaw, 0.0f);
        }

        /// <summary>
        /// Get The current device status if it's keyboard or gamepad
        /// </summary>
        public bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        public void GrabTransformation()
        {
            if (!TimeOverSinceGrab)
            {
                TimeSinceGrab += 2.0f * Time.deltaTime;
                if (TimeSinceGrab >= 0.65f)
                {
                    TimeOverSinceGrab = true;
                }
            }

            // 1. Focus Mode Position & Rotation
            // Directly checks _inputs.focus
            if (_inputs.focus && TimeOverSinceGrab)
            {
                _rb.interpolation = RigidbodyInterpolation.None;

                // Ensure it lerps toward objectGrabPointTransformOnFocus
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransformOnFocus.position,
                    lerpGrab * Time.deltaTime);
                transform.position = targetPosition;

                // Store current rotation for when we exit focus
                _currentRotation = Quaternion.Inverse(_maincamera.transform.rotation) * transform.rotation;
                return;
            }

            // 2. Normal hold mode (Snapped after initial pull timer complete)
            if (TimeOverSinceGrab)
            {
                _rb.interpolation = RigidbodyInterpolation.None;
                transform.position = objectGrabPointTransform.position;
            }
            // 3. Initial pull towards hand
            else
            {
                _rb.interpolation = RigidbodyInterpolation.Interpolate;
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position,
                    lerpGrab * Time.deltaTime);
                transform.position = targetPosition;
            }

            // Apply rotation relative to camera for normal holding
            transform.rotation = _maincamera.transform.rotation * _currentRotation;
        }

        public void RigidBodyGrabSetting(bool isGrabbed)
        {
            if (isGrabbed)
            {
                _rb.linearDamping = 5.0f;
                _rb.angularDamping = 5.0f;
            }
            else
            {
                _rb.linearDamping = 0.0f;
                _rb.angularDamping = 0.0f;
            }
        }

        public void MoveInventory()
        {
            if (!_inputs.inventoryMove) return;
            //This was causing race condition previously 
            _inputs.inventoryMove = false;

            //Only works when inventorymove is pressed
            // Search this object AND its children/parents
            ItemObject item = GetComponentInChildren<ItemObject>();
            if (item == null)
            {
                item = GetComponentInParent<ItemObject>();
            }

            if (item != null) item.OnHandlePickupItem();
        }

        #endregion

        #region Awake

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        #endregion

        #region Start/Update

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //cheaky method to fix the rotation of the grabbable object (Error 0 * t = 0 will make things 0) now fixed it with 1 * t = t
            _currentRotation = Quaternion.Euler(1.0f, 1.0f, 1.0f);
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.linearDamping = 0.0f;
            _rb.angularDamping = 0.0f;
            _rb.freezeRotation = false;
            _rb.isKinematic = false;
            _rb.detectCollisions = true;
            _inputs = FindFirstObjectByType<StarterAssetsInputs>();
            _playerRigidBodyPush = FindFirstObjectByType<BasicRigidBodyPush>();
            //This is a method to find gameObject transform in runtime so you don't have to manually put things on your own
            GameObject targetTransform = GameObject.FindGameObjectWithTag("OnFocusGrabPoint");
            if (targetTransform != null) objectGrabPointTransformOnFocus = targetTransform.transform;
#if ENABLE_INPUT_SYSTEM
            _playerInput = FindFirstObjectByType<PlayerInput>();
#endif
            _maincamera = FindFirstObjectByType<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // Early exit: if this object is sitting in the world ungrabbed, skip all math!
            if (objectGrabPointTransform == null) return;
            RotateGrabbableObjectOnFocus();
            GrabTransformation();
            MoveInventory();
        }

        #endregion
    }
}