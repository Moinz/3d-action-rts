using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace CM.Units
{
    public class UnitLocomotion : MonoBehaviour
    {
        [SerializeField] 
        private NavMeshAgent _agent;
        
        public Action onArrived;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        
        public void Stop()
        {
            _agent.isStopped = true;

            onArrived?.Invoke();
        }
        
        public void MoveTo(Vector3 position, Action onArrived = null)
        {
            _agent.SetDestination(position);
            StartCoroutine(WaitForArrivalCoroutine());

            this.onArrived = onArrived;
        }

        private IEnumerator WaitForArrivalCoroutine()
        {
            yield return null;

            while (_agent.remainingDistance > .05f)
            {
                ChangeOrientation();
                
                yield return null;
            }

            Debug.Log(gameObject.name + " Arrived!");
            onArrived?.Invoke();
        }

        public bool IsMoving()
        {
            return _agent.velocity.magnitude > 0;
        }

        public void ChangeOrientation()
        {
            if (_agent.velocity.magnitude > 0)
            {
                var direction = _agent.velocity.normalized;
                var lookRotation = Quaternion.LookRotation(direction);
                
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
            }
        }
    }
}