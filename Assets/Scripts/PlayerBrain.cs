using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CM.Units
{
    [System.Serializable]
    public class PlayerBrain : Brain, PlayerControls.IInteractionActions 
    {
        private PlayerControls _playerControls;
        
        private IInteractable _interactable;
        
        private States _currentState;
        
        private enum States
        {
            Idle,
            Moving,
            Interacting,
        }
        
        public PlayerBrain(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;
        }
        
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            var mousePos = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(mousePos);
            var hit = Physics.Raycast(ray, out var hitInfo, 1000f, searchMask, QueryTriggerInteraction.Collide);
            
            if (!hit || !hitInfo.rigidbody)
                return;
            
            var hasInteractable = hitInfo.rigidbody.TryGetComponent(out _interactable);
            _currentState = States.Moving;
            _unitController.MoveTo(hitInfo.point, () =>
            {
                if (!hasInteractable) 
                    return;
                
                _interactable.Interact(_unitController);
                _currentState = States.Interacting;
            });
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
        }

        public override int TickRate => 30;
        
        public override void Tick()
        {
            switch (_currentState)
            {
                case States.Idle:
                    break;
                case States.Moving:
                    break;
                case States.Interacting:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Idle_State()
        {
            
        }

        private void Idle_Moving()
        {
            
        }
        
        private void Idle_Interacting()
        {
            if (_interactable == null)
            {
                _currentState = States.Idle;
                return;
            }
            
            _interactable.Interact(_unitController);
        }

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;

            _playerControls = new PlayerControls();
            _playerControls.Enable();
            
            _playerControls.Interaction.SetCallbacks(this);
            _playerControls.Interaction.Enable();
        }
    }
}