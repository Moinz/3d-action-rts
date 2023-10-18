using UnityEngine;
using UnityEngine.InputSystem;

namespace CM.Units
{
    [System.Serializable]
    public class PlayerBrain : Brain, PlayerControls.IInteractionActions 
    {
        private PlayerControls _playerControls;
        
        private IInteractable _interactable;
        
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
            
            _unitController.MoveTo(hitInfo.point, () =>
            {
                if (hasInteractable)
                    _interactable.Interact(_unitController);
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