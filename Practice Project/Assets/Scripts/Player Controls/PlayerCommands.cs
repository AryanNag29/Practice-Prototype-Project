using UnityEngine;

namespace PrototypeProject
{
    /*#region Command Pattern
    //The blueprint for all commands. every command must know how to execute itself
    public interface IPlayerCommand
    {
        void Execute(ThirdPersonController body);
    }
    //Command: "Walk/Run" in this direction
    public struct MoveCommand : IPlayerCommand
    {
        public Vector2 MoveInput;
        public bool IsSprinting;

        public MoveCommand(Vector2 moveInput, bool isSprinting)
        {
            MoveInput = moveInput;
            IsSprinting = isSprinting;
        }

        public void Execute(ThirdPersonController body)
        {
            //the command tells the body what to do, passing along the data
            body.ProcessMovement(MoveInput, IsSprinting);
        }
    }

    public struct JumpCommand : IPlayerCommand
    {
        public bool JumpPressed;

        public JumpCommand(bool jumpPressed)
        {
            JumpPressed = jumpPressed;
        }

        public void Execute(ThirdPersonController body)
        {
            body.ProcessJumpAndGravity(JumpPressed);
        }
    }

    public struct LookCommand : IPlayerCommand
    {
        public Vector2 LookInput;
        public bool IsFocusing;
        public bool IsMouseDevice;

        public LookCommand(Vector2 lookInput, bool isFocusing, bool isMouseDevice)
        {
            LookInput = lookInput;
            IsFocusing = isFocusing;
            IsMouseDevice = isMouseDevice;
        }

        public void Execute(ThirdPersonController body)
        {
            body.ProcessCameraRotation(LookInput, IsFocusing, IsMouseDevice);
        }
    }

    #endregion*/
}