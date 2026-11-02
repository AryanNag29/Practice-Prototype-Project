using StarterAssets;
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
        [Tooltip("The physical body this brain will control")] 
        [SerializeField] private ThirdPersonController _body;

        [SerializeField] private StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();
            if (_body == null) _body = GetComponent<ThirdPersonController>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
        }

        private void Update()
        {
            if (_body == null) return;

            bool isFocusing = (_input.pickDrop && _input.focus);
            Vector2 safeMoveInput = isFocusing ? Vector2.zero : _input.move;

            // 2. CREATE COMMANDS
            IPlayerCommand jumpCommand = new JumpCommand(_input.jump);
    
            // FIX: Consume the jump input immediately so it doesn't get stuck on true
            _input.jump = false; 

            IPlayerCommand moveCommand = new MoveCommand(safeMoveInput, _input.sprint);

            // 3. EXECUTE COMMANDS
            jumpCommand.Execute(_body);
            moveCommand.Execute(_body);
        }

        void LateUpdate()
        {
            if (_body == null) return;
        
            bool isFocusing = (_input.pickDrop && _input.focus);
        
            // FIX: Default to true so mouse input doesn't get crushed by Time.deltaTime
            bool isMouse = true; 

#if ENABLE_INPUT_SYSTEM
            if (_playerInput != null)
            {
                isMouse = _playerInput.currentControlScheme == "KeyboardMouse";
            }
#endif
        
            // DIAGNOSTIC LOG
            if (_input.look != Vector2.zero)
            {
                Debug.Log($"CAMERA IS WORKING: Sending Look {_input.look} | isMouse: {isMouse}");
            }

            IPlayerCommand lookCommand = new LookCommand(_input.look, isFocusing, isMouse);
            lookCommand.Execute(_body);
        }
    }
}