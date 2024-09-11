using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace DGP.StackableInput.Editor.Tests
{
    internal class MyMockGameInputClass : IInputActionCollection {
        public IEnumerator<InputAction> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(InputAction action) {
            return false;
        }

        public void Enable() {
            //noop
        }

        public void Disable() {
            //noop
        }

        public InputBinding? bindingMask { get; set; }
        public ReadOnlyArray<InputDevice>? devices { get; set; }
        public ReadOnlyArray<InputControlScheme> controlSchemes { get; }
    }

    internal class MyMockGameMode : IGameInputMode<MyMockGameInputClass>
    {
        public void EnterGameInputMode(MyMockGameInputClass gameInput) {
            
        }

        public void ExitGameInputMode(MyMockGameInputClass gameInput) {
            
        }
    }

    internal class MyMockGameInputSystem : PlayerInputService<MyMockGameInputClass, MyMockGameMode>
    {
        public MyMockGameInputSystem(MyMockGameInputClass gameInput) : base(gameInput) {
            
        }
    }
    
    public class StackableInputTests
    {
        [Test]
        public void TestPlayerInputService()
        {
            var gameInput = new MyMockGameInputClass();
            var gameMode = new MyMockGameMode();
            var playerInputService = new MyMockGameInputSystem(gameInput);
            
            Assert.AreEqual(playerInputService.CurrentInputMode, null);
            
            playerInputService.PushInputMode(gameMode);
            
            Assert.AreEqual(playerInputService.CurrentInputMode, gameMode);
            
            playerInputService.PopInputMode();
            
            Assert.AreEqual(playerInputService.CurrentInputMode, null);
        }
          
    }
}
