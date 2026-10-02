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
        public bool pickDrop;

        [Header("Movement Settings")] public bool analogMovement;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        
        //vector 2 movement 
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
        
        //tap button
        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnTpp(InputValue value)
        {
            TppInput(value.isPressed);

        }

        public void OnFpp(InputValue value)
        {
            FppInput(value.isPressed);
        }

        public void OnPickDrop(InputValue value)
        {
            if (value.isPressed)
            {
                PickDropInput();
            }
        }
        
        
        //hold button
        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnFocus(InputValue value)
        {
            FocusInput(value.isPressed);
        }
#endif

        //vector 2 movement 
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }
        
        //tap button
        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void TppInput(bool newTppState)
        {
            tpp = newTppState;
        }
        

        public void FppInput(bool newFppState)
        {
            fpp = newFppState;
        }

        public void PickDropInput()
        {
            pickDrop = !pickDrop;
        }
        
        //hold button
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