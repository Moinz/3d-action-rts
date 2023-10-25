using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CM.Units
{
    [System.Serializable]
    public class BoarBrain : Brain
    {
        public override int TickRate => 10;

        private Vector3 _originalPosition;

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;
            
            _originalPosition = _stateController.transform.position;
        }

        private enum BoarStates
        {
            Patrol,
            Approach,
            Attacking,
        }

        private BoarStates _currentBoarState;

        public override void Tick()
        {
            switch (_currentBoarState)
            {
                case BoarStates.Patrol:
                    Patrol();
                    return;
                case BoarStates.Approach:
                    Approach();
                    return;
                case BoarStates.Attacking:
                    Attack();
                    return;
            }
        }

        private void Attack()
        {
            if (!Target)
            {
                _currentBoarState = BoarStates.Patrol;
                return;
            }
            
            _unitController.Attack?.Invoke(Target);
        }

        private void Approach()
        {
            if (!Target)
                return;
            
            var targetPos = Target.transform.position;
            var distance = Vector3.Distance(_stateController.transform.position, targetPos);

            if (distance < _unitController.interactRange)
            {
                _currentBoarState = BoarStates.Attacking;
                _unitController.Stop();
            }
        }

        private void Patrol()
        {
            SelectEnemy();

            if (!Target)
            {
                Wander();
                return;
            }

            _unitController.MoveTo(Target.transform.position);
            _currentBoarState = BoarStates.Approach;
        }

        private void SelectEnemy()
        {
            Target = null;
            
            _stateController.Query_PerformSearch(searchMask);
            if (_stateController.PrunedAndSortedColliders.Count == 0)
                return;

            foreach (var sortedCollider in _stateController.PrunedAndSortedColliders)
            {
                var rb = sortedCollider.attachedRigidbody;
                if (!rb.TryGetComponent(out UnitController unitController)) 
                    continue;
                
                if (_unitController == unitController)
                    continue;
                
                Target = unitController.gameObject;
                return;
            }
        }

        private void Wander()
        {
            var pos = _originalPosition;
            var randomPos = RandomPointInCircle(pos, 3f);
            
            _unitController.MoveTo(randomPos, OnArrived);
            _currentBoarState = BoarStates.Approach;
        }

        private void OnArrived()
        {
            _currentBoarState = BoarStates.Patrol;
        }
        
        // TODO: Move this to a utility class.
        private Vector3 RandomPointInCircle(Vector3 center, float radius)
        {
            var point = Random.insideUnitCircle * radius;
        
            return center + new Vector3(point.x, 0f, point.y);
        }

    }
}