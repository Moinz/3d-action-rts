using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(menuName = "CM/Behaviors/Brains/Soldier")]
    public class SoldierBrain : Brain
    {
        public override int TickRate => 10;
        
        private enum CombatStates
        {
            Patrol,
            Approach,
            Attacking,
        }

        private CombatStates _currentCombatState;

        public override void Tick()
        {
            switch (_currentCombatState)
            {
                case CombatStates.Patrol:
                    Patrol();
                    return;
                case CombatStates.Approach:
                    Approach();
                    return;
                case CombatStates.Attacking:
                    Attack();
                    return;
            }
        }

        private void Attack()
        {
            if (!_stateController.target)
            {
                _currentCombatState = CombatStates.Patrol;
                return;
            }
            
            _unitController.Attack?.Invoke(Target);
        }

        private void Approach()
        {
            var targetPos = Target.transform.position;
            var distance = Vector3.Distance(_stateController.transform.position, targetPos);

            if (distance < _unitController.interactRange)
            {
                _currentCombatState = CombatStates.Attacking;
                _unitController.Stop();
            }
        }

        private void Patrol()
        {
            _stateController.Query_PerformSearch(searchMask);

            if (_stateController.PrunedAndSortedColliders.Count == 0)
                return;
            
            SelectTarget();
            
            if (!Target)
                return;

            _unitController.MoveTo(Target.transform.position);
            _currentCombatState = CombatStates.Approach;
        }

        private void SelectTarget()
        {
            Target = null;
            
            foreach (var sortedCollider in _stateController.PrunedAndSortedColliders)
            {
                var rb = sortedCollider.attachedRigidbody;
                if (!rb.TryGetComponent(out UnitController unitController)) 
                    continue;

                // If we have the same _isEnemy setting, we are probably friends.
                // TODO: A more sophisticated way of determining friend or foe.
                if (unitController._isEnemy == _unitController._isEnemy) 
                    continue;
                
                Target = unitController.gameObject;
                return;
            }
        }

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;
        }
    }
}