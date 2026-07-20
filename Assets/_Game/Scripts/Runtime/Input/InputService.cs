using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Runtime.Input
{
    public class InputService : IDisposable
    {
        private readonly InputSystemActions _actions = new();

        private readonly Subject<Unit> _onRotateItem = new();
        private readonly Subject<int> _setSpeed = new();

        public Observable<Unit> OnRotateItem => _onRotateItem;
        public Observable<int> SetSpeed => _setSpeed;

        public InputService()
        {
            Subscribe();
        }

        public void Enable()
        {
            _actions.Player.Enable();
            _actions.UI.Enable();
            _actions.Inventory.Enable();
        }

        public void Disable() 
        {
            _actions.Player.Disable();
            _actions.UI.Disable();
            _actions.Inventory.Disable();
        }

        private void Subscribe()
        {
            _actions.Inventory.RotateItem.performed += HandleRotate;
            _actions.Inventory.SetSpeed.performed += HandleSpeedSet;
        }

        private void UnSubscribe()
        {
            _actions.Inventory.RotateItem.performed -= HandleRotate;
            _actions.Inventory.SetSpeed.performed -= HandleSpeedSet;
        }

        private void HandleRotate(InputAction.CallbackContext context) 
            => _onRotateItem.OnNext(Unit.Default);


        private void HandleSpeedSet(InputAction.CallbackContext context)
        {
            int speedLevel = Mathf.RoundToInt(context.ReadValue<float>());
            _setSpeed.OnNext(speedLevel);
        }

        public void Dispose()
        {
            UnSubscribe();
        }
    }
}
