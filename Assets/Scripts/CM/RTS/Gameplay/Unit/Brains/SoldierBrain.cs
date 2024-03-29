using System;
using TriInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CM.Units
{
    [Serializable]
    public class SoldierBrain : Brain
    {
        public override int TickRate => 10;
        
        private enum CombatStates
        {
            Patrol,
            Approach,
            Attacking,
        }

        [ShowInInspector]
        private CombatStates _currentCombatState;

        [ShowInInspector]
        private float _timeUntilNextWander;
        
        private Vector3 _originalPosition;

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
            if (!Target)
            {
                _currentCombatState = CombatStates.Patrol;
                return;
            }
            
            _unitController.Attack?.Invoke(Target);
        }

        private void Approach()
        {
            if (!Target)
            {
                _currentCombatState = CombatStates.Patrol;
                return;
            }
            
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
            {
                Wander();
                return;
            }
            
            SelectTarget();

            if (!Target)
            {
                Wander();
                return;
            }

            _unitController.MoveTo(Target.transform.position);
            _currentCombatState = CombatStates.Approach;
        }

        
        private bool CanWander()
        {
            if (_timeUntilNextWander > Time.time)
                return false;
            
            _timeUntilNextWander = Time.time + Random.Range(3f, 6f);
            return true;
        }
        
        private void Wander()
        {
            if (!CanWander())
                return;
            
            var pos = _originalPosition;
            var randomPos = BrainExtensions.RandomPointInCircle(pos, 3f);
            
            _unitController.MoveTo(randomPos, OnArrived);
            _currentCombatState = CombatStates.Patrol;
        }

        private void OnArrived()
        {
            
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
                if (unitController.team == _unitController.team) 
                    continue;
                
                Target = unitController.gameObject;
                return;
            }
        }

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;
            
            _originalPosition = _stateController.transform.position;
            _timeUntilNextWander = Time.time + Random.Range(3f, 6f);
        }
    }
}