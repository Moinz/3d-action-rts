﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CM.Units
{
    public partial class UnitStateController : MonoBehaviour
    {
        private UnitController _unitController;
        public GameObject target;

        private Brain _activeBrain;

        public void Initialize(Brain brain)
        {
            _unitController = GetComponent<UnitController>();

            _activeBrain = brain;
            _activeBrain.Initialize(this, _unitController);
        }

        private void Update()
        {
            if (Time.frameCount % _activeBrain.TickRate != 0)
                    return;
            
            _activeBrain.Tick();
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
    }
}