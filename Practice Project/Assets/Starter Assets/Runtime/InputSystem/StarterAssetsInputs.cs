using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PrototypeProject
{
    [System.Serializable]
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool focus;
        public bool fpp;
        public bool tpp;

        [Header("Movement Settings")] public bool analogMovement;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnTpp(InputValue value)
        {
            if (value.isPressed)
            {
                TppInput();
            }
        }

        public void OnFpp(InputValue value)
        {
            FppInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnFocus(InputValue value)
        {
            FocusInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void TppInput()
        {
            tpp = !tpp;
        }

        public void FppInput(bool newFppState)
        {
            fpp = newFppState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void FocusInput(bool newFocusState)
        {
            focus = newFocusState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}