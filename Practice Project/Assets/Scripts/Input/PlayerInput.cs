using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PrototypeProject
{
    public class PlayerInput : MonoBehaviour
    {
        #region Variables

        [Header("Player Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Setting")] public bool analogMovement;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;
        
        #endregion

        #region Functions

    

        #endregion
        
    }
}