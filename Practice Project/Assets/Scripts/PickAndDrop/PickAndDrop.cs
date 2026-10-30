using System;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrototypeProject
{
    public static class InteractionEvents
    {
        // Fired when camera raycast enters/exits grabbable range
        public static event Action<ObjectGrabbable> OnGrabbableInRange;
        public static event Action OnGrabbableOutOfRange;

        // Fired when player presses inputs while in range
        public static event Action<DoorInteracttion> OnDoorInteractRequested;
        public static event Action<ObjectGrabbable, Transform> OnGrabRequested;
        public static event Action OnDropRequested;
        
        //Trigger event
        public static void GrabbableInRange(ObjectGrabbable target) => OnGrabbableInRange?.Invoke(target);
        public static void GrabbableOutOfRange() => OnGrabbableOutOfRange?.Invoke();
        public static void RequestDoorInteract(DoorInteracttion door) => OnDoorInteractRequested?.Invoke(door);

        public static void RequestGrab(ObjectGrabbable target, Transform grabPoint) =>
            OnGrabRequested?.Invoke(target, grabPoint);

        public static void RequestDrop() => OnDropRequested?.Invoke();
    }

    //in this we are going to use raycast from camera to object and see if it hits objects
    public class PickAndDrop : MonoBehaviour
    {
        #region References

        [SerializeField] private Transform _objectGrabPointTransform;
        [SerializeField] private Renderer _outliner;
        private Renderer _currentRenderer;

        [SerializeField] private Transform _mainCamera;

        /*[SerializeField] private FPPTOTPP _cameraChange;*/
        [SerializeField] private LayerMask _pickObjectMask;
        private StarterAssetsInputs _inputs;
        private ObjectGrabbable _objectGrabbable;

        //Cached shader property ID to avoid string hashing overhead every frame
        private static readonly int OutlinerScaleId = Shader.PropertyToID("_OutlinerScale");

        #endregion

        #region Variables

        [Header("Distance")] [SerializeField] private float _interactDistance = 3.0f;
        [SerializeField] private float _pickUpDistance = 2.0f;

        //Track camera mode purely through events(No direct FPPTOTPP script dependency needed!)
        private CameraViewMode _currentViewMode = CameraViewMode.ThirdPerson;

        #endregion

        #region Event Subscription

        private void OnEnable()
        {
            CameraEvent.OnViewRequested += HandleViewModeChanged;
        }

        private void OnDisable()
        {
            CameraEvent.OnViewRequested -= HandleViewModeChanged;
        }

        private void HandleViewModeChanged(CameraViewMode newMode)
        {
            _currentViewMode = newMode;

            //if player switch to tpp from fpp while grabbing any object drop it
            if (_currentViewMode == CameraViewMode.ThirdPerson)
            {
                if (_inputs != null)
                {
                    //make interact and grab false when tpp
                    _inputs.interactiveObject = false;
                    _inputs.pickDrop = false;
                }

                ResetOutliner(_currentRenderer);

                if (_objectGrabbable != null)
                {
                    InteractionEvents.RequestDrop();
                    _objectGrabbable = null;
                }
            }
        }

        #endregion

        #region Functions

        private void OnHitInteractable()
        {
            if (_currentViewMode == CameraViewMode.ThirdPerson)
            {
                _inputs.interactiveObject = false;
                ResetOutliner(_currentRenderer);
                return;
            }

            if (_currentViewMode == CameraViewMode.FirstPerson)
            {
                float interactDistance = 3f;
                if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out RaycastHit hit, interactDistance))
                {
                    //door is detected
                    if (hit.transform.TryGetComponent(out DoorInteracttion doorDetected))
                    {
                        if (_inputs.interactiveObject)
                        {
                            //Fire Door event instade of starting coroutine directly in the script
                            InteractionEvents.RequestDoorInteract(doorDetected);
                            
                            //ones toggle make it false
                            _inputs.interactiveObject = false;
                        }
                    }
                    else
                    {
                        //hitting other object will not trigger it
                        _inputs.interactiveObject = false;
                    }
                }
                else
                {
                    //Hitting air will not trigger it
                    _inputs.interactiveObject = false;
                }
            }
        }

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
            if (_currentViewMode == CameraViewMode.ThirdPerson)
            {
                _inputs.pickDrop = false;
                ResetOutliner(_currentRenderer);
            }

            if (_currentViewMode == CameraViewMode.FirstPerson)
            {
                if (_objectGrabbable == null)
                {
                    // Not carrying the object, try to grab
                    float pickUpDistance = 2f;
                    if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out RaycastHit hit,
                            pickUpDistance,
                            _pickObjectMask))
                    {
                        // Use a local variable to detect instead of overwriting _objectGrabbable
                        if (hit.transform.TryGetComponent(out ObjectGrabbable detectedGrabbable))
                        {
                            _outliner = hit.collider.GetComponent<Renderer>();

                            // If we hovered over a different object, clear the old outline first
                            if (_currentRenderer != null && _currentRenderer != _outliner)
                            {
                                ResetOutliner(_currentRenderer);
                            }
                            //Only update material & fire range event ones per new hovered object
                            if (_outliner != null && _outliner.sharedMaterials.Length > 1)
                            {
                                Material[] _OutMat = _outliner.materials;
                                _OutMat[1].SetFloat(OutlinerScaleId, 1.1f);
                                _outliner.materials = _OutMat;

                                _currentRenderer = _outliner;
                                InteractionEvents.GrabbableInRange(detectedGrabbable);
                            }

                            if (_inputs.pickDrop)
                            {
                                _objectGrabbable = detectedGrabbable;
                                //no need to do this now directly
                                /*_objectGrabbable.Grab(_objectGrabPointTransform);*/
                                ResetOutliner(_currentRenderer);
                                
                                //Fire Grab event instead of calling _objectGrabbable.Grab() Directly
                                InteractionEvents.RequestGrab(_objectGrabbable,_objectGrabPointTransform);
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
                        _inputs.pickDrop = false;
                    }
                }
                else if (!_inputs.pickDrop)
                {
                    // Currently carrying something, drop
                    InteractionEvents.RequestDrop();
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

            if (targetRenderer.sharedMaterials.Length > 1)
            {
                Material[] mats = targetRenderer.materials;
                mats[1].SetFloat(OutlinerScaleId, 0f);
                targetRenderer.materials = mats;
            }

            _currentRenderer = null;
            InteractionEvents.GrabbableOutOfRange();
        }

        private void OnDrawGizmos()
        {
            if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out RaycastHit hit,
                    2f,
                    _pickObjectMask))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_mainCamera.position, hit.point);
            }
        }

        #endregion


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _inputs = GetComponent<StarterAssetsInputs>();
        }

        // Update is called once per frame
        void Update()
        {
            OnHitInteractable();
            OnHitGrabbable();
        }
    }
}