using System.Linq;
using UnityEngine;

namespace CM.Units
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private Resource[] _resources = new Resource[6];
        
        public int Capacity => _resources.Length;
        public int Contents => _resources.Count(x => x != null);
        public bool IsFull => Contents == Capacity;

        public bool TryAddResource(Resource resource)
        {
            for (int i = 0; i < _resources.Length; i++)
            {
                if (_resources[i] == null)
                {
                    _resources[i] = resource;
                    
                    resource.transform.SetParent(transform);
                    resource.gameObject.SetActive(false);
                    return true;
                }
            }

            return false;
        }
        
        public bool TryRemoveResource(Resource resource)
        {
            for (int i = 0; i < _resources.Length; i++)
            {
                if (_resources[i] == resource)
                {
                    _resources[i] = null;
                    return true;
                }
            }

            return false;
        }

        public void DepositResources(Stockpile stockpile)
        {
            stockpile.Deposit(Contents);

            for (var index = 0; index < _resources.Length; index++)
            {
                var resource = _resources[index];
                _resources[index] = null;

                if (resource)
                    resource.transform.SetParent(stockpile.transform);
            }
        }
    }
}