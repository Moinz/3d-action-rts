using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CM.Units
{
    public partial class UnitStateController : EntityBehavior
    {
        private UnitController _unitController;
        public GameObject target;

        [SerializeField]
        private BrainData _brainData;

        private bool _initialized;

        public void Initialize(UnitArchetype archetype)
        {
            _unitController = GetComponent<UnitController>();
            _unitController.Initialize(archetype);
            
            _brainData = archetype.brainData;
            _brainData.Initialize(this, _unitController);
            
            _initialized = true;
        }
        
        private void Update()
        {
            if (!_initialized)
                return;

            if (ConnectedEntity.Status.Value != EntityStatus.Alive)
                return;
            
            if (target && !target.gameObject.activeInHierarchy)
                target = null;
            
            if (_brainData)
                _brainData.Brain.Tick();
        }

        private void OnDrawGizmos()
        {
            if (!_unitController)
                _unitController = GetComponent<UnitController>();
            
            Gizmos.DrawWireSphere(transform.position, _unitController.vision);
        }
    }

    public partial class UnitStateController
    {
        private Collider[] SearchColliders = new Collider[20];
        public List<Collider> PrunedAndSortedColliders = new();   
        
        public void Query_PerformSearch(LayerMask layerMask)
        {
            SearchColliders = new Collider[20];
            
            var pos = transform.position;
            var hits = Physics.OverlapSphereNonAlloc(pos, _unitController.vision, SearchColliders, layerMask,
                QueryTriggerInteraction.Ignore);
            
            if (hits == 0)
                return;
            
            var colliders = Prune(SearchColliders.ToList());
            PrunedAndSortedColliders = colliders.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList();
        }
        
        public List<T> Prune<T>(List<T> list) where T : Collider
        {
            // reverse for loop to remove items from list
            for (int i = list.Count - 1; i >= 0; i--)
            {
                // if item is null, remove from list
                if (list[i] == null)
                    goto removeAndContinue;

                var entity = list[i].attachedRigidbody.GetComponent<Entity>();

                if (!entity)
                    goto removeAndContinue;
                
                // if item is owned by the same entity as us, remove from list
                if (entity == ConnectedEntity)
                    goto removeAndContinue;
                
                if (entity.Status.Value == EntityStatus.Alive)
                    continue;
                
                removeAndContinue:
                list.RemoveAt(i);
            }

            return list;
        }
    }
}