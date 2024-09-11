using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace DGP.StackableInput
{
    public abstract class PlayerInputService<TInputActionAsset, TGameInputMode> where TInputActionAsset : IInputActionCollection where TGameInputMode : class, IGameInputMode<TInputActionAsset>
    {
        private readonly TInputActionAsset gameInput;
        private readonly Stack<TGameInputMode> inputModeStack = new();

        protected PlayerInputService(TInputActionAsset gameInput) {
            this.gameInput = gameInput;
        }

        ~PlayerInputService() {
            gameInput.Disable();
            
            while (inputModeStack.Count > 0)
                PopInputMode();
        }
        
        public TGameInputMode CurrentInputMode => inputModeStack.Count > 0 ? inputModeStack.Peek() : null;
        
        public void PushInputMode(TGameInputMode nextMode) {
            if (inputModeStack.TryPeek(out TGameInputMode currentMode))
                currentMode.ExitGameInputMode(gameInput);
            
            inputModeStack.Push(nextMode);
            nextMode.EnterGameInputMode(gameInput);
        }
        
        public void PopInputMode() {
            if (inputModeStack.TryPeek(out TGameInputMode currentMode)) {
                currentMode.ExitGameInputMode(gameInput);
                inputModeStack.Pop();
            }
            
            if (inputModeStack.TryPeek(out TGameInputMode nextMode))
                nextMode.EnterGameInputMode(gameInput);
        }
    }
}