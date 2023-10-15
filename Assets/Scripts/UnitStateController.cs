using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CM.Units
{
    public class UnitStateController : MonoBehaviour
    {
        private UnitController _controller;
        private Inventory _inventory;
        
        public GameObject target;
        
        private Collider[] SearchColliders = new Collider[20];
        public List<Collider> PrunedAndSortedColliders = new();
        
        public Inventory Inventory => _inventory;

        [SerializeField]
        private GathererBrain _gathererBrain;
        
        [SerializeField]
        private SoldierBrain _soldierBrain;
        
        [SerializeField]
        private bool _isGatherer;

        private void Start()
        {
            _controller = GetComponent<UnitController>();
            _inventory = GetComponent<Inventory>();

            _gathererBrain.Initialize(this, _controller);
            _soldierBrain.Initialize(this, _controller);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _isGatherer = !_isGatherer;
            
            // Gatherer
            if (_isGatherer)
            {
                if (Time.frameCount % _gathererBrain.TickRate != 0)
                    return;
                
                _gathererBrain.Tick();
                return;
            }
            
            // Soldier
            if (Time.frameCount % _soldierBrain.TickRate != 0)
                return;
            
            _soldierBrain.Tick();
            
        }

        public List<T> Prune<T>(List<T> list)
        {
            // reverse for loop to remove items from list
            for (int i = list.Count - 1; i >= 0; i--)
            {
                // if item is null, remove from list
                if (list[i] == null)
                    list.RemoveAt(i);
            }

            return list;
        }

        private void OnDrawGizmos()
        {
            if (!_controller)
                _controller = GetComponent<UnitController>();
            
            Gizmos.DrawWireSphere(transform.position, _controller.vision);
        }
        
        public void Query_PerformSearch(LayerMask layerMask)
        {
            SearchColliders = new Collider[20];
            
            var pos = transform.position;
            var hits = Physics.OverlapSphereNonAlloc(pos, _controller.vision, SearchColliders, layerMask,
                QueryTriggerInteraction.Ignore);
            
            if (hits == 0)
                return;
            
            var colliders = Prune(SearchColliders.ToList());
            PrunedAndSortedColliders = colliders.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList();
        }
    }
}