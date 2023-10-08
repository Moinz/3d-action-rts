using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace CM.Units
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField]
        private NavMeshAgent _agent;

        [SerializeField]
        private Attachment[] _attachments;

        private Action onArrived;
        
        public float vision;
        
        public Action<GameObject> Attack;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _attachments = GetComponentsInChildren<Attachment>();
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
    }
}