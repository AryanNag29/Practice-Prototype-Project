using System;
using UnityEngine;
using Unity.Cinemachine;
using Unity.Mathematics;
namespace PrototypeProject
{
    public class FPPTOTPP : MonoBehaviour
    {
        #region Reference

        private CinemachineCamera _camera;
        private CinemachineThirdPersonFollow _follow;
        //lol now i understand why the fpp to cpp can't get this _input because it's on different folder and i was accessing it with get component method
        [SerializeField] private StarterAssetsInputs _inputs;

        #endregion

        #region Variables

        //present state of camera
        public bool Fpp = false;
        public bool Tpp = true;
        private float fovlerpValue = 10.0f;

        //camera distance
        [SerializeField] private float tppDistance = 3.0f;
        [SerializeField] private float fppDistance = -0.5f;

        //Fov variables
        [SerializeField] private float fovInFpp = 60.0f;
        [SerializeField] private float fovInTpp = 40.0f;
        [SerializeField] private float sprintFovFpp = 80.0f;
        [SerializeField] private float sprintFovTpp = 60.0f;
        [SerializeField] private float focusFovFpp = 20f;
        
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
            //value transfer
            sprintInput = _inputs.sprint;
            focusInput = _inputs.focus;
            
            // sprintInput = _inputs.sprint;
            if (_follow.CameraDistance == tppDistance)
            {
                if (_inputs.fpp)
                {
                    _camera.Lens.FieldOfView = fovInFpp;
                    _follow.CameraDistance = fppDistance;
                    Fpp = true;
                    Tpp = false;
                }
                //make the inputs false ones button is used
                _inputs.fpp = false;
            }

            if (_follow.CameraDistance == fppDistance)
            {
                if (_inputs.tpp)
                {
                    _camera.Lens.FieldOfView = fovInTpp;
                    _follow.CameraDistance = tppDistance;
                    Fpp = false;
                    Tpp = true;
                }

                _inputs.tpp = false;
            }

            //bug here (can't access the _input.sprint)
            if (Fpp)
            {
                if (sprintInput)
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, sprintFovFpp, fovlerpValue * Time.deltaTime);
                }
                else
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, fovInFpp, fovlerpValue * Time.deltaTime);
                }
            }

            if (Fpp)
            {
                if (focusInput)
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, focusFovFpp, fovlerpValue * Time.deltaTime);
                }
                else
                {
                        _camera.Lens.FieldOfView =
                            Mathf.Lerp(_camera.Lens.FieldOfView, fovInFpp, fovlerpValue * Time.deltaTime);
                }
            }

            if (Tpp)
            {
                if (sprintInput)
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, sprintFovTpp, fovlerpValue * Time.deltaTime);
                }
                else
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, fovInTpp, fovlerpValue * Time.deltaTime);
                }
            }
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