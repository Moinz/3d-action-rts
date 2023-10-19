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
        public int Count(ResourceSO resourceType) => _resources.Count(x => x != null && x.SO == resourceType);

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
            if (resource == null)
                return false;
            
            for (int i = 0; i < _resources.Length; i++)
            {
                if (_resources[i] == resource)
                {
                    _resources[i] = null;
                    resource.transform.parent = null;
                    resource.gameObject.SetActive(true);
                    return true;
                }
            }

            return false;
        }

        public void DepositResources(Stockpile stockpile)
        {
            var resourceType = stockpile.resource;
            stockpile.Deposit(Count(resourceType));

            for (var index = 0; index < _resources.Length; index++)
            {
                var resource = _resources[index];
                if (!resource || resource.SO != resourceType)
                    continue;
                
                TryRemoveResource(resource);
                resource.transform.SetParent(stockpile.transform);
                resource.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        public Resource GetResource(ResourceSO targetResource)
        {
            foreach (var resource in _resources)
            {
                if (!resource)
                    continue;
                
                if (resource.SO == targetResource)
                    return resource;
            }

            return null;
        }
        
        public void EjectInventory()
        {
            foreach (var resource in _resources)
            {
                if (!resource)
                    continue;
                
                TryRemoveResource(resource);
                resource.transform.position += Vector3.up;
                ResourceFactory.ThrowRandom(resource.gameObject, resource.transform.position, 2.5f);
            }
        }
    }
}