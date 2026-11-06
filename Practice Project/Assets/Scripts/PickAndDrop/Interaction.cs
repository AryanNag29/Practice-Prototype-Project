using System;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrototypeProject
{
    /// <summary>
    /// Events which will go on as player interact
    /// Add new events as player have more interactive system
    /// </summary>
    public static class InteractionEvents
    {
        // Fired when camera raycast enters/exits grabbable range
        public static event Action<ObjectGrabbable> OnGrabbableInRange;
        public static event Action OnGrabbableOutOfRange;

        // Fired when player presses inputs while in range
        public static event Action<ObjectGrabbable, Transform> OnGrabRequested;
        public static event Action OnDropRequested;

        //Fired when camera raycast Enters/exits Door Interaction range
        public static event Action<DoorInteracttion> OnDoorInteractionInRange;
        public static event Action OnDoorInteractionOutOfRange;

        //Door interaction event
        public static event Action<DoorInteracttion> OnDoorInteractRequested;

        // inventory system event
        public static event Action<ItemObject> OnInventoryStoreRequested;

        //Trigger event

        //Grabbable object range Trigger
        public static void GrabbableInRange(ObjectGrabbable target) => OnGrabbableInRange?.Invoke(target);

        //Grabbable object out of range Trigger
        public static void GrabbableOutOfRange() => OnGrabbableOutOfRange?.Invoke();

        //Grab trigger
        public static void RequestGrab(ObjectGrabbable target, Transform grabPoint) =>
            OnGrabRequested?.Invoke(target, grabPoint);

        //drop trigger
        public static void RequestDrop() => OnDropRequested?.Invoke();

        //Door Interaction range tigger

        public static void DoorInRange(DoorInteracttion target) => OnDoorInteractionInRange?.Invoke(target);

        public static void DoorOutOfRange() => OnDoorInteractionOutOfRange?.Invoke();


        //Door interaction trigger
        public static void RequestDoorInteract(DoorInteracttion door) => OnDoorInteractRequested?.Invoke(door);

        //Inventory store Trigger
        public static void RequestInventoryStore(ItemObject item)
        {
            OnInventoryStoreRequested?.Invoke(item);
        }
    }

    //in this we are going to use raycast from camera to object and see if it hits objects
    public class Interaction : MonoBehaviour
    {
        #region References

        [SerializeField] private Transform _objectGrabPointTransform;
        [SerializeField] private Renderer _outliner;

        //Create different renderer for different outliner
        private Renderer _currentGrabRenderer;
        private Renderer _currentDoorRenderer;

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
            //Request for camera event enable
            CameraEvent.OnViewRequested += HandleViewModeChanged;
        }

        private void OnDisable()
        {
            //Request for camera event Disable
            CameraEvent.OnViewRequested -= HandleViewModeChanged;
        }

        /// <summary>
        /// Handle camera mode if it's tpp or fpp and set the interactive and pickup input according to it
        /// </summary>
        /// <param name="newMode"></param>
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

                ResetOutlinerGrabbable(_currentGrabRenderer);

                if (_objectGrabbable != null)
                {
                    InteractionEvents.RequestDrop();
                    _objectGrabbable = null;
                }
            }
        }

        #endregion

        #region Functions

        private void OnHitInteractableDoor()
        {
            // compare with camera view event if it's Third person
            if (_currentViewMode == CameraViewMode.ThirdPerson)
            {
                _inputs.interactiveObject = false;
                ResetOutlinerDoor(_currentDoorRenderer);
            }

            // compare with camera view event if it's First person
            if (_currentViewMode == CameraViewMode.FirstPerson)
            {
                float interactDistance = 3f;
                if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out RaycastHit hit, interactDistance))
                {
                    if (hit.transform.TryGetComponent(out DoorInteracttion doorDetected))
                    {
                        _outliner = hit.collider.GetComponentInChildren<Renderer>();

                        // If we hovered over a different object, clear the old outline first
                        if (_currentDoorRenderer != null && _currentDoorRenderer != _outliner)
                        {
                            Debug.Log("_current render != null");
                            ResetOutlinerGrabbable(_currentDoorRenderer);
                        }

                        //Only update material & fire range event ones per new hovered object
                        if (_outliner != null && _outliner.sharedMaterials.Length > 1)
                        {
                            Material[] _OutMat = _outliner.materials;
                            _OutMat[1].SetFloat(OutlinerScaleId, 1.1f);
                            _outliner.materials = _OutMat;

                            _currentDoorRenderer = _outliner;
                            InteractionEvents.DoorInRange(doorDetected);
                        }

                        if (_inputs.interactiveObject)
                        {
                            InteractionEvents.RequestDoorInteract(doorDetected);
                            ResetOutlinerDoor(_currentDoorRenderer);
                            _currentDoorRenderer = null; // FORGET THE DOOR AFTER INTERACTING
                            _inputs.interactiveObject = false;
                        }
                    }
                    else
                    {
                        //hitting other object will not trigger it
                        _inputs.interactiveObject = false;
                        ResetOutlinerDoor(_currentDoorRenderer);
                    }
                }
                else
                {
                    //Hitting air will not trigger it
                    _inputs.interactiveObject = false;
                    ResetOutlinerDoor(_currentDoorRenderer);
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
                ResetOutlinerGrabbable(_currentGrabRenderer);
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
                            if (_currentGrabRenderer != null && _currentGrabRenderer != _outliner)
                            {
                                ResetOutlinerGrabbable(_currentGrabRenderer);
                            }

                            //Only update material & fire range event ones per new hovered object
                            if (_outliner != null && _outliner.sharedMaterials.Length > 1)
                            {
                                Material[] _OutMat = _outliner.materials;
                                _OutMat[1].SetFloat(OutlinerScaleId, 1.1f);
                                _outliner.materials = _OutMat;

                                _currentGrabRenderer = _outliner;
                                InteractionEvents.GrabbableInRange(detectedGrabbable);
                            }

                            if (_inputs.pickDrop)
                            {
                                _objectGrabbable = detectedGrabbable;
                                //no need to do this now directly
                                /*_objectGrabbable.Grab(_objectGrabPointTransform);*/
                                ResetOutlinerGrabbable(_currentGrabRenderer);

                                //Fire Grab event instead of calling _objectGrabbable.Grab() Directly
                                InteractionEvents.RequestGrab(_objectGrabbable, _objectGrabPointTransform);
                            }
                        }
                        else
                        {
                            // Hit something that cannot be grabbed
                            ResetOutlinerGrabbable(_currentGrabRenderer);
                        }
                    }
                    else
                    {
                        // Raycast hit nothing in the air
                        ResetOutlinerGrabbable(_currentGrabRenderer);
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
        /// Create different resetoutliner for different objects
        /// </summary>
        /// <param name="targetRenderer"></param>
        private void ResetOutlinerGrabbable(Renderer targetRenderer)
        {
            if (targetRenderer == null) return;

            if (targetRenderer.sharedMaterials.Length > 1)
            {
                Material[] mats = targetRenderer.materials;
                mats[1].SetFloat(OutlinerScaleId, 0f);
                targetRenderer.materials = mats;
            }

            _currentGrabRenderer = null;
            InteractionEvents.GrabbableOutOfRange();
        }

        /// <summary>
        /// This function is to reset the outliner when raycast hitting air or non Door object
        /// Create different resetoutliner for different objects
        /// </summary>
        /// <param name="targetRenderer"></param>
        private void ResetOutlinerDoor(Renderer targetRenderer)
        {
            if (targetRenderer == null) return;

            if (targetRenderer.sharedMaterials.Length > 1)
            {
                Material[] mats = targetRenderer.materials;
                mats[1].SetFloat(OutlinerScaleId, 0f);
                targetRenderer.materials = mats;
            }

            _currentDoorRenderer = null;
            InteractionEvents.DoorOutOfRange();
        }

        /// <summary>
        /// Gizmos to check if the ray is falling on the interactive object
        /// </summary>
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
            OnHitInteractableDoor();
            OnHitGrabbable();
        }
    }
}