using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CM.Units
{
    [Serializable]
    public class PlayerBrain : Brain, PlayerControls.IInteractionActions
    {
        private PlayerControls _playerControls;
        private SelectionController _selectionController;
        
        private States _currentState;
        
        private enum States
        {
            Idle,
            Moving,
            Interacting,
        }

        public PlayerBrain()
        {
            
        }
        
        public PlayerBrain(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;
        }

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;

            _selectionController = GameObject.FindObjectOfType<SelectionController>();

            _playerControls = new PlayerControls();
            _playerControls.Enable();
            
            _playerControls.Interaction.SetCallbacks(this);
            _playerControls.Interaction.Enable();
        }
        
        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            _selectionController.Select();
        }
        
        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            MoveToCursor();
        }


        public override int TickRate => 30;
        
        public override void Tick()
        {
            switch (_currentState)
            {
                case States.Idle:
                    Idle_State();
                    return;
                case States.Moving:
                    Moving_State();
                    return;
                case States.Interacting:
                    Interacting_State();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Idle_State()
        {
            if (!_selectionController.HasSelection) 
                return;
            
            MoveToSelection();
        }

        private void Moving_State()
        {
            
        }
        
        private void Interacting_State()
        {
            if (!_selectionController.HasSelection)
            {
                _currentState = States.Idle;
                return;
            }
            
            var interactable = _selectionController.Selected as IInteractable;
            if (interactable == null)
            {
                _currentState = States.Idle;
                return;
            }

            if (Vector3.Distance(interactable.gameObject.transform.position, _unitController.transform.position) >
                _unitController.interactRange)
            {
                MoveToSelection();
                return;
            }
            
            interactable.Interact(_unitController);
        }

        
        private void MoveToSelection()
        {
            _currentState = States.Moving;
            _unitController.MoveTo(_selectionController.Selected.Rigidbody.position, () =>
            {
                _currentState = States.Interacting;
            });
        }

        private void MoveToCursor()
        {
            _currentState = States.Moving;

            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Default"));
            
            _unitController.MoveTo(hit.point, () =>
            {
                _currentState = States.Idle;
            });
        }
    }
}