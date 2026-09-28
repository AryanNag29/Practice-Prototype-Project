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
        [SerializeField]private StarterAssetsInputs _inputs;

        #endregion

        #region Variables
        //present state of camera
        [SerializeField] private bool Fpp = true;
        [SerializeField] private bool Tpp = true;
        //camera distance
        [SerializeField] private float tppDistance = 3.0f;
        [SerializeField] private float fppDistance = -0.5f;
        //Fov variables
        [SerializeField] private float fovInFpp = 60.0f;
        [SerializeField] private float fovInTpp = 40.0f;
        [SerializeField] private float sprintFov = 80.0f;

        #endregion

        #region Function

        public void FPPTOCPP()
        {
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
                    Fpp = true;
                    Tpp = false;
                }
            }

            if (Fpp)
            {
                if (_inputs.sprint)
                {
                    _camera.Lens.FieldOfView =
                        Mathf.Lerp(_camera.Lens.FieldOfView, fovInFpp, sprintFov * Time.deltaTime);
                }
            }
        }

        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _follow = GetComponent<CinemachineThirdPersonFollow>();
            _camera = GetComponent<CinemachineCamera>();
            _inputs = GetComponent<StarterAssetsInputs>();
        }

        // Update is called once per frame
        void Update()
        {
            FPPTOCPP();
        }
    }
}