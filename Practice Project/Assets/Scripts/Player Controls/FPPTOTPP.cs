using System;
using UnityEngine;
using Unity.Cinemachine;
using Unity.Mathematics;

namespace PrototypeProject
{
    //enum: replaces the twin bools (FPP/TPP) to prevent invalid states
    public enum CameraViewMode
    {
        FirstPerson,
        ThirdPerson
    }

    //Global Event channel so any script can request camera/focus transition
    public static class CameraEvent
    {
        //Evemt for camera view mode of type enum
        public static event Action<CameraViewMode> OnViewRequested;

        //Event for is camera focusing of bool type
        public static event Action<bool> OnFocusRequested;

        //Assigning
        public static void SetViewMode(CameraViewMode mode) => OnViewRequested?.Invoke(mode);
        public static void SetFocus(bool isFocusing) => OnFocusRequested?.Invoke(isFocusing);
    }

    [RequireComponent(typeof(CinemachineCamera))]
    [RequireComponent(typeof(CinemachineThirdPersonFollow))]
    public class FPPTOTPP : MonoBehaviour
    {
        #region Reference

        [Header("References")] private CinemachineCamera _camera;

        private CinemachineThirdPersonFollow _follow;

        //lol now i understand why the fpp to cpp can't get this _input because it's on different folder and i was accessing it with get component method
        [SerializeField] private StarterAssetsInputs _inputs;

        #endregion

        #region Variables

        //present state of camera
        public bool Fpp => _currentMode == CameraViewMode.FirstPerson;
        public bool Tpp => _currentMode == CameraViewMode.ThirdPerson;
        [SerializeField] private CameraViewMode _currentMode = CameraViewMode.ThirdPerson;

        private float fovlerpValue = 10.0f;
        [SerializeField] private float distanceLerpValue = 15.0f;

        //camera distance
        [SerializeField] private float tppDistance = 3.0f;
        [SerializeField] private float fppDistance = -0.5f;

        //Fov variables
        [SerializeField] private float fovInFpp = 60.0f;
        [SerializeField] private float fovInTpp = 40.0f;
        [SerializeField] private float sprintFovFpp = 80.0f;
        [SerializeField] private float sprintFovTpp = 60.0f;
        [SerializeField] private float focusFovFpp = 40f;

        //Inputs
        [SerializeField] private bool sprintInput;
        [SerializeField] private bool focusInput;

        #endregion

        #region Function

        /// <summary>
        /// FppToTpp Function: this function switch camera to fpp to tpp and tpp to fpp
        /// storing sprint bool input into sprint to see if player is sprinting (This is for fov change when player runs)
        /// storing focusinput into focus bool (this is for fov change when player use focus)
        /// change into fpp
        /// change into tpp
        /// fpp sprint fov change logic 
        /// fpp focus logic(focus will only work on fpp mode)
        /// tpp sprint fov change logic
        /// </summary>
        public void FppTOTpp()
        {
            if (_inputs == null) return;

            //value transfer
            sprintInput = _inputs.sprint;
            focusInput = _inputs.focus;

            if (_inputs.fpp)
            {
                CameraEvent.SetViewMode(CameraViewMode.FirstPerson);
                _inputs.fpp = false;
            }
            else if (_inputs.tpp)
            {
                CameraEvent.SetViewMode(CameraViewMode.ThirdPerson);
                _inputs.tpp = false;
            }

            //Target Calculation & smooth interpolation
            UpdateCameraOptics();
        }
        
        /// <summary>
        /// put camera optics
        /// </summary>
        private void UpdateCameraOptics()
        {
            //determine target distance based on current mode if fpp then fpp distance if not tppdistance
            float targetDisatance = (_currentMode == CameraViewMode.FirstPerson) ? fppDistance : tppDistance;

            float targetFov = ResloveTargetFov();

            _follow.CameraDistance =
                Mathf.Lerp(_follow.CameraDistance, targetDisatance, distanceLerpValue * Time.deltaTime);

            _camera.Lens.FieldOfView = Mathf.Lerp(_camera.Lens.FieldOfView, targetFov, fovlerpValue * Time.deltaTime);
        }

        /// <summary>
        /// Resolve TargetFov
        /// The fov will be one float value at a time right ? so why not one function of float type which return that one fov value at different condition
        /// if in fpp mode and focus is press return focus fov fpp
        /// if in fpp mode and sprint is press it will return sprint fov also only sprint when player is not in still 
        /// if just in fpp then it will return fov in fpp
        /// same for tpp
        /// </summary>
        /// <returns></returns>
        private float ResloveTargetFov()
        {
            //if fpp then it's logic
            if (_currentMode == CameraViewMode.FirstPerson)
            {
                if (focusInput)
                {
                    return focusFovFpp;
                }

                //fpp sprint fov change logic

                if (sprintInput && MoveInputReturn())
                {
                    return sprintFovFpp;
                }

                return fovInFpp;
            }
            else //tpp
            {
                if (sprintInput && MoveInputReturn())
                {
                    return sprintFovTpp;
                }

                return fovInTpp;
            }
        }
        
        
        /// <summary>
        /// moveinputreturn of type bool
        /// if input = null then return false
        /// return input.move sqrmagnitude > 0,001f;
        /// </summary>
        /// <returns></returns>
        public bool MoveInputReturn()
        {
            if (_inputs == null) return false;
            return _inputs.move.sqrMagnitude > 0.001f;
        }

        #endregion

        #region OnEnable/Disable

        private void HandleViewModeRequested(CameraViewMode mode) => _currentMode = mode;
        private void HandleFocusRequested(bool isFocusing) => focusInput = isFocusing;

        private void OnEnable()
        {
            CameraEvent.OnViewRequested += HandleViewModeRequested;
            CameraEvent.OnFocusRequested += HandleFocusRequested;
        }

        private void OnDisable()
        {
            CameraEvent.OnViewRequested -= HandleViewModeRequested;
            CameraEvent.OnFocusRequested -= HandleFocusRequested;
        }

        #endregion

        private void Awake()
        {
            //this is useful for many situation remember it {Also it get object in runtime}
            //fixed it if the get component is not working because object disappear in runtime just use this 
            _inputs = FindFirstObjectByType<StarterAssetsInputs>();
        }

        void Start()
        {
            _follow = GetComponent<CinemachineThirdPersonFollow>();
            _camera = GetComponent<CinemachineCamera>();
        }


        void Update()
        {
            FppTOTpp();
        }
    }
}