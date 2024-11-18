using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace DGP.StackableInput
{
    public abstract class PlayerInputService<TInputActionAsset, TGameInputMode> : IDisposable where TInputActionAsset : IInputActionCollection where TGameInputMode : class, IGameInputMode<TInputActionAsset>
    {
        private bool _disposed;
        
        protected readonly TInputActionAsset GameInput;
        private readonly Stack<TGameInputMode> _inputModeStack = new();

        protected PlayerInputService(TInputActionAsset gameInput) {
            this.GameInput = gameInput;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
        
            if (disposing)
            {
                GameInput.Disable();
                while (_inputModeStack.Count > 0)
                    PopInputMode();
            }
        
            _disposed = true;
        }
        
        public TGameInputMode CurrentInputMode => _inputModeStack.Count > 0 ? _inputModeStack.Peek() : null;
        
        public void PushInputMode(TGameInputMode nextMode) {
            if (nextMode == null)
                throw new ArgumentNullException(nameof(nextMode));
            
            if (!ValidateInputModeChange(nextMode))
                return;
            
            OnBeforePushInputMode(nextMode);
            
            if (_inputModeStack.TryPeek(out TGameInputMode currentMode)) {
                currentMode.ExitGameInputMode(GameInput);
                OnInputModeExited(currentMode);
            }
            
            _inputModeStack.Push(nextMode);
            nextMode.EnterGameInputMode(GameInput);
            OnInputModeEntered(nextMode);
            
            OnAfterPushInputMode(nextMode);
        }
        
        public void PopInputMode() {
            if (!_inputModeStack.TryPeek(out TGameInputMode currentMode))
                return;
            
            if (!ValidateInputModePop(currentMode))
                return;
            
            OnBeforePopInputMode(currentMode);
            
            currentMode.ExitGameInputMode(GameInput);
            OnInputModeExited(currentMode);
            _inputModeStack.Pop();
            
            if (_inputModeStack.TryPeek(out TGameInputMode nextMode)) {
                nextMode.EnterGameInputMode(GameInput);
                OnInputModeEntered(nextMode);
            }
            
            OnAfterPopInputMode(currentMode);
        }
        
        #region Extension Points
        /// <summary>
        /// Validates whether the specified input mode can be pushed onto the stack.
        /// </summary>
        /// <param name="nextMode">The mode being pushed</param>
        /// <returns>True if the mode can be pushed, false otherwise</returns>
        protected virtual bool ValidateInputModeChange(TGameInputMode nextMode) => true;

        /// <summary>
        /// Validates whether the current input mode can be popped from the stack.
        /// </summary>
        /// <param name="currentMode">The mode being popped</param>
        /// <returns>True if the mode can be popped, false otherwise</returns>
        protected virtual bool ValidateInputModePop(TGameInputMode currentMode) => true;

        /// <summary>
        /// Called before a new input mode is pushed onto the stack.
        /// </summary>
        protected virtual void OnBeforePushInputMode(TGameInputMode nextMode) { }

        /// <summary>
        /// Called after a new input mode has been pushed onto the stack.
        /// </summary>
        protected virtual void OnAfterPushInputMode(TGameInputMode nextMode) { }

        /// <summary>
        /// Called before the current input mode is popped from the stack.
        /// </summary>
        protected virtual void OnBeforePopInputMode(TGameInputMode currentMode) { }

        /// <summary>
        /// Called after an input mode has been popped from the stack.
        /// </summary>
        protected virtual void OnAfterPopInputMode(TGameInputMode previousMode) { }

        /// <summary>
        /// Called when an input mode becomes active.
        /// </summary>
        protected virtual void OnInputModeEntered(TGameInputMode mode) { }

        /// <summary>
        /// Called when an input mode becomes inactive.
        /// </summary>
        protected virtual void OnInputModeExited(TGameInputMode mode) { }
        #endregion
    }
}