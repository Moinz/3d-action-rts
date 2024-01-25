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

            while (_agent.remainingDistance > .25f)
            {
                yield return null;
            }

            Debug.Log(gameObject.name + " Arrived!");
            onArrived?.Invoke();
        }

        public bool IsMoving()
        {
            return _agent.velocity.magnitude > 0;
        }
    }
}