using Unity.VisualScripting;
using UnityEngine;

namespace PrototypeProject
{
    //in this we are going to use raycast from camera to object and see if it hits objects
    public class PickAndDrop : MonoBehaviour
    {
        #region References

        [SerializeField] private Transform _objectGrabPointTransform;
        [SerializeField] private Renderer _outliner;
        private Renderer _currentRenderer;
        [SerializeField] private Transform _mainCamera;
        [SerializeField] private FPPTOTPP _cameraChange;
        [SerializeField] private LayerMask _pickObjectMask;
        private StarterAssetsInputs _inputs;
        private ObjectGrabbable _objectGrabbable;

        #endregion

        #region Variables

        #endregion

        #region Functions

        /// <summary>
        /// On Hit Grabbable Function
        /// condition: player should be in fpp mode
        /// condition: player pickdrop key should be true
        /// condition: object grabbable should be null (can't pick two objects)
        /// ray cast from camera to the object which contain grabbable script
        /// Condition: if object grabbable is not null and pickdrop key is false then drop object
        /// </summary>
        private void OnHitGrabbable()
        {
            if (_cameraChange.Tpp)
            {
                _inputs.pickDrop = false;
            }

            if (_cameraChange.Fpp)
            {
                if (_objectGrabbable == null)
                {
                    //Not carrying the object try to grab
                    float pickUpDistance = 2f;
                    if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out RaycastHit hit,
                            pickUpDistance,
                            _pickObjectMask))
                    {
                        //only respond if that object have object grabbable script 
                        if (hit.transform.TryGetComponent(out _objectGrabbable))
                        {
                            Debug.Log(_objectGrabbable);
                            //This is to get the material component
                            _outliner = hit.collider.GetComponent<Renderer>();
                            if (_outliner != null && _outliner.sharedMaterials.Length > 1)
                            {
                                //shared gameobject material
                                Material[] _OutMat = _outliner.materials;

                                _OutMat[1].SetFloat("_OutlinerScale", 1.1f);

                                _outliner.materials = _OutMat;

                                _currentRenderer = _outliner;
                            }

                            if (_inputs.pickDrop)
                            {
                                _objectGrabbable.Grab(_objectGrabPointTransform);
                            }
                        }
                        else
                        {
                            // Hit something that cannot be grabbed
                            ResetOutliner(_currentRenderer);
                        }
                    }
                    else
                    {
                        // Raycast hit nothing in the air
                        ResetOutliner(_currentRenderer);
                        //if hit air then you can't press pickup
                        _inputs.pickDrop = false;
                    }
                }

                if (_objectGrabbable != null && !_inputs.pickDrop)
                {
                    //currently carrying something, drop
                    _objectGrabbable.Drop();
                    _objectGrabbable = null;
                }
            }
        }

        /// <summary>
        /// This function is to reset the outliner when raycast hitting air or non grabbable object
        /// </summary>
        /// <param name="targetRenderer"></param>
        private void ResetOutliner(Renderer targetRenderer)
        {
            if (targetRenderer == null) return;

            if (targetRenderer != null && targetRenderer.sharedMaterials.Length > 1)
            {
                Material[] mats = targetRenderer.materials;
                mats[1].SetFloat("_OutlinerScale", 0f);
                targetRenderer.materials = mats;
            }

            _currentRenderer = null;
        }

        #endregion


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _cameraChange = FindFirstObjectByType<FPPTOTPP>();
            _inputs = GetComponent<StarterAssetsInputs>();
            //Material is under Renderer
        }

        // Update is called once per frame
        void Update()
        {
            //if player switch to tpp from fpp while grabbing any object drop it
            if (_objectGrabbable != null && _cameraChange.Tpp)
            {
                _inputs.pickDrop = false;
                _objectGrabbable.Drop();
                _objectGrabbable = null;
            }

            OnHitGrabbable();
        }
    }
}