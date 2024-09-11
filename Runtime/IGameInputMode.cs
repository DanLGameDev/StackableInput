using UnityEngine.InputSystem;

namespace DGP.StackableInput
{
    public interface IGameInputMode<TInputActionAsset> where TInputActionAsset : IInputActionCollection
    {
        public void EnterGameInputMode(TInputActionAsset gameInput);
        public void ExitGameInputMode(TInputActionAsset gameInput);
    }
}