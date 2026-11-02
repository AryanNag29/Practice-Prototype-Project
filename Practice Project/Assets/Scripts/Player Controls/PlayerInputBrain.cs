using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PrototypeProject
{
    [RequireComponent(typeof(StarterAssetsInputs))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerInputBrain : MonoBehaviour
    {
        [Tooltip("The physical body this brain will control")] [SerializeField]
        private ThirdPersonController _body;

        private StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();

            //Auto-Assign the body if you forget to drag it to the inspector
            if (_body == null) _body = GetComponent<ThirdPersonController>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}