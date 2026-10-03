using Unity.VisualScripting;
using UnityEngine;

namespace PrototypeProject
{
    //in this we are going to use raycast from camera to object and see if it hits objects
    public class PickAndDrop : MonoBehaviour
    {
        #region References

        [SerializeField] private Transform _mainCamera;
        [SerializeField] private FPPTOTPP _cameraChange;
        [SerializeField] private LayerMask _pickObjectMask;
        private StarterAssetsInputs _inputs;

        #endregion

        #region Variables

        #endregion

        #region Functions

        private void OnHitGrabbable()
        {
            if (_cameraChange.Fpp)
            {
                if (_inputs.pickDrop)
                {
                    float pickUpDistance = 2f;
                    if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out RaycastHit hit, pickUpDistance,
                            _pickObjectMask))
                    {
                        //only respond if that object have object grabbable script 
                        if (hit.transform.TryGetComponent(out ObjectGrabbable objectGrabbable))
                        {
                            Debug.Log(objectGrabbable);
                        }
                    }
                }
            }
        }

        #endregion


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _cameraChange = FindFirstObjectByType<FPPTOTPP>();
            _inputs = GetComponent<StarterAssetsInputs>();
        }

        // Update is called once per frame
        void Update()
        {
            OnHitGrabbable();
        }
    }
}