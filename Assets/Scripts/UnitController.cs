using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace CM.Units
{
    public class UnitController : MonoBehaviour
    {
        public bool _isEnemy;
        
        [SerializeField]
        private NavMeshAgent _agent;

        [SerializeField]
        private Attachment[] _attachments;

        private HealthModule _healthModule;
        internal Inventory _inventory;
        
        private Action onArrived;
        
        public float vision;
        
        public Action<GameObject> Attack;
        
        public float interactRange = 2.5f;


        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _healthModule = GetComponent<HealthModule>();

            _inventory = GetComponent<Inventory>();
            _attachments = GetComponentsInChildren<Attachment>();
        }

        private void OnEnable()
        {
            if (_healthModule)
                _healthModule.OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            if (_healthModule)
                _healthModule.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            _inventory.EjectInventory();
            
            gameObject.SetActive(false);
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
            
            Debug.Log(gameObject.name +" Arrived!");
            onArrived?.Invoke();
        }

        public Attachment GetAttachment(Attachment.Enum_AttachmentType type)
        {
            foreach (var attachment in _attachments)
            {
                if (attachment.attachmentType == type)
                    return attachment;
            }

            return null;
        }

        public void Stop()
        {
            _agent.isStopped = true;
            
            onArrived?.Invoke();
        }
    }
}