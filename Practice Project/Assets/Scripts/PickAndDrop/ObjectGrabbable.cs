using System;
using System.IO.Enumeration;
using UnityEngine;

namespace PrototypeProject
{
    public class ObjectGrabbable : MonoBehaviour
    {
        #region References

        private Rigidbody _rb;
        private Transform objectGrabPointTransform;
        private Transform objectGrabPointTransformOnFocus;
        private StarterAssetsInputs _inputs;

        #endregion

        #region Variables

        private float lerpGrab = 20.0f;
        #endregion

        #region Functions
        
        /// <summary>
        /// This is a Grab function for the player to Grab object when the "E" key is pressed
        /// Rigid body Kinematic is on
        /// rigid body detect collision is off
        /// gravity is off
        /// Storing object grab point transform position into the grabbable object
        /// </summary>
        /// <param name="objectGrabPointTransform"></param>
        public void Grab(Transform objectGrabPointTransform)
        {
            _rb.isKinematic = true;
            _rb.detectCollisions = false;
            _rb.useGravity = false; 
            this.objectGrabPointTransform = objectGrabPointTransform;
        }

        /// <summary>
        /// This is a drop function for the player to drop object when the "E" key is pressed
        /// Rigid body Kinematic is off
        /// rigid body detect collision is on
        /// gravity is on
        /// restoring grabbable object to it's current position
        /// </summary>
        public void Drop()
        {
            this.objectGrabPointTransform = null;
            _rb.useGravity = true;
            _rb.detectCollisions = true;
            _rb.isKinematic = false;
        }

        #endregion

        #region Awake/Fixed Update

        private void FixedUpdate()
        {
            if (objectGrabPointTransform != null && !_inputs.focus)
            {
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, lerpGrab * Time.deltaTime);
                _rb.MovePosition(targetPosition);
            }

            if (objectGrabPointTransform != null && _inputs.focus)
            {
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransformOnFocus.position, lerpGrab * Time.deltaTime);
                _rb.MovePosition(targetPosition);
            }
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
            _rb.detectCollisions = true;
            _rb.isKinematic = false;
            _inputs = FindFirstObjectByType<StarterAssetsInputs>();
            // //This is a method to find gameObject transform in runtime so you don't have to manually put things on your own
            // GameObject targetTransform = GameObject.FindGameObjectWithTag("OnFocusGrabPoint");
            // objectGrabPointTransformOnFocus = targetTransform.transform;
        }

        // Update is called once per frame
        void Update()
        {
        }
        #endregion
    }
}