using System;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

namespace PrototypeProject
{
    public class FPPTOTPP : MonoBehaviour
    {
        #region Reference

        private CinemachineCamera _camera;
        private CinemachineThirdPersonFollow _follow;
        [SerializeField] private StarterAssetsInputs _inputs;

        #endregion

        #region Variables

        //present state of camera
        private bool Fpp = false;
        private bool Tpp = true;

        //camera distance
        [SerializeField] private float tppDistance = 3.0f;
        [SerializeField] private float fppDistance = -0.5f;

        //Fov variables
        [SerializeField] private float fovInFpp = 60.0f;
        [SerializeField] private float fovInTpp = 40.0f;
        [SerializeField] private float sprintFovFpp = 80.0f;
        [SerializeField] private float sprintFovTpp = 60.0f;
        [SerializeField] private float fovlerpValue = 0.5f;

        //Inputs
        [SerializeField] private bool sprintInput;

        #endregion

        #region Function

        public void FPPTOCPP()
        {
            sprintInput = _inputs.sprint;
            // sprintInput = _inputs.sprint;
            if (_follow.CameraDistance == tppDistance)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    _camera.Lens.FieldOfView = fovInFpp;
                    _follow.CameraDistance = fppDistance;
                    Fpp = true;
                    Tpp = false;
                }
            }

            if (_follow.CameraDistance == fppDistance)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    _camera.Lens.FieldOfView = fovInTpp;
                    _follow.CameraDistance = tppDistance;
                    Fpp = false;
                    Tpp = true;
                }
            }

            //bug here (can't access the _input.sprint)
            if (Fpp)
            {
                if (sprintInput)
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, sprintFovFpp, fovInFpp * fovlerpValue * Time.deltaTime);
                }
                else
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, fovInFpp, sprintFovFpp * fovlerpValue * Time.deltaTime);
                }
            }

            if (Tpp)
            {
                if (sprintInput)
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, sprintFovTpp, fovInTpp * fovlerpValue * Time.deltaTime);
                }
                else
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, fovInTpp, sprintFovTpp * fovlerpValue * Time.deltaTime);
                }
            }
        }

        #endregion

        private void Awake()
        {
            //this is useful for many situation remember it {Also it get object in runtime}
            //fixed it if the get component is not workind because object disappear in runtime just use this 
            _inputs = FindFirstObjectByType<StarterAssetsInputs>();
        }

        void Start()
        {
            _follow = GetComponent<CinemachineThirdPersonFollow>();
            _camera = GetComponent<CinemachineCamera>();
        }


        void Update()
        {
            FPPTOCPP();
        }
    }
}