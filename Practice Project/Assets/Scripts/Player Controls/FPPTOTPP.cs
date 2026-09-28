using Unity.Cinemachine;
using UnityEngine;

namespace PrototypeProject
{
    public class FPPTOTPP : MonoBehaviour
    {
        #region Reference

        private CinemachineCamera _camera;
        private CinemachineThirdPersonFollow _follow;

        #endregion

        #region Variables

        private float tppDistance = 3.0f;

        #endregion

        #region Function

        public void FPPTOCPP()
        {
            if (_follow.CameraDistance == 3.0f)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    _camera.Lens.FieldOfView = 60f;
                    _follow.CameraDistance = -0.5f;
                }
            }

            if (_follow.CameraDistance == -0.5f)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    _camera.Lens.FieldOfView = 40f;
                    _follow.CameraDistance = tppDistance;
                }
            }
        }

        #endregion

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _follow = GetComponent<CinemachineThirdPersonFollow>();
            _camera = GetComponent<CinemachineCamera>();
        }

        // Update is called once per frame
        void Update()
        {
            FPPTOCPP();
        }
    }
}