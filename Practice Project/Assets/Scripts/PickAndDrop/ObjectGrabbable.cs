using System;
using UnityEngine;

namespace PrototypeProject
{
    public class ObjectGrabbable : MonoBehaviour
    {
        #region References

        private Rigidbody _rb;
        private Transform objectGrabPointTransform;

        #endregion

        #region Variables

        #endregion

        #region Functions

        public void Grab(Transform objectGrabPointTransform)
        {
            _rb.isKinematic = true;
            _rb.detectCollisions = false;
            _rb.useGravity = false; 
            this.objectGrabPointTransform = objectGrabPointTransform;
        }

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
            if (objectGrabPointTransform != null)
            {
                Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, 20f * Time.deltaTime);
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
        }

        // Update is called once per frame
        void Update()
        {
        }
        #endregion
    }
}