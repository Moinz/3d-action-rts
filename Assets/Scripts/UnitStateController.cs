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
        private List<Collider> PrunedAndSortedColliders = new();

        [SerializeField] 
        private LayerMask layerMask;

        [SerializeField]
        private ResourceSO _targetResourceSO;
        
        [SerializeField]
        private Enum_States _state;
        
        private enum Enum_States
        {
            Idle,
            Searching,
            Moving,
            PickUpAttachment,
            PickUpResource,
            Harvesting,
            Returning
        }

        private void Start()
        {
            _controller = GetComponent<UnitController>();
            _inventory = GetComponent<Inventory>();
        }
        
        private void Update()
        {
            if (Time.frameCount % 30 != 0)
                return;
            
            switch (_state)
            {
                case Enum_States.Idle:
                    _state = Enum_States.Searching;
                    State_Idle();
                    return;
                case Enum_States.Searching:
                    State_Searching();
                    return;
                case Enum_States.Moving:
                    State_Moving();
                    return;
                case Enum_States.Harvesting:
                    State_Harvesting();
                    return;
                case Enum_States.Returning:
                    State_Returning();
                    return;
                default:
                    State_Idle();
                    return;
            }
        }

        private void State_Idle()
        {
        }

        private void State_Searching()
        {
            Query_PerformSearch();
            
            var weapon = _controller.GetAttachment(Attachment.Enum_AttachmentType.Weapon).HasAttachment;
            if (!weapon)
            {
                Debug.Log(gameObject.name + "No Weapon, finding one.");
                FindAttachment(Attachment.Enum_AttachmentType.Weapon);
                return;
            }

            var shield = _controller.GetAttachment(Attachment.Enum_AttachmentType.Shield).HasAttachment;
            if (!shield)
            {
                Debug.Log(gameObject.name + "No Shield, finding one.");
                FindAttachment(Attachment.Enum_AttachmentType.Shield);
                return;
            }

            if (FindAvailableResource())
                return;
            
            Debug.Log(gameObject.name + "Searching for resource");
            FindClosestResourceNode();
        }

        private Attachment.Enum_AttachmentType _targetAttachment;
        
        private void FindAttachment(Attachment.Enum_AttachmentType type)
        {
            Debug.Log(gameObject.name + " Find Attachment : " + type);
            _targetAttachment = type;
            
            var equipments = FindObjectsByType<Equipment>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var equipment in equipments)
            {
                if (equipment.attachmentType != type || equipment.IsEquipped)
                    continue;
                
                Debug.Log(gameObject.name +" Found Attachment : " + type + " : " + equipment);
                
                target = equipment.gameObject;
                _state = Enum_States.PickUpAttachment;
                _controller.MoveTo(equipment.transform.position, OnArrived);
                break;
            }
        }
        
        private void Query_PerformSearch()
        {
            var pos = transform.position;
            var hits = Physics.OverlapSphereNonAlloc(pos, _controller.vision, SearchColliders, layerMask,
                QueryTriggerInteraction.Ignore);
            
            if (hits == 0)
                return;
            
            var colliders = Prune(SearchColliders.ToList());
            PrunedAndSortedColliders = colliders.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList();
        }

        private bool FindAvailableResource()
        {
            var weapon = _controller.GetAttachment(Attachment.Enum_AttachmentType.Weapon).attachedEquipment as Weapon;
            _targetResourceSO = weapon.resource;
            
            Debug.Log("Looking for resource (" + _targetResourceSO.name + ")");
            
            for (int i = 0; i < PrunedAndSortedColliders.Count; i++)
            {
                var hit = PrunedAndSortedColliders[i];

                var rb = hit.attachedRigidbody;
                
                if (!rb)
                    continue;
                
                if (!rb.TryGetComponent(out Resource resource)) 
                    continue;
                
                if (resource.SO != _targetResourceSO)
                    continue;

                Debug.Log("Found resource (" + target + ")");
                
                target = resource.gameObject;
                _state = Enum_States.PickUpResource;
                _controller.MoveTo(resource.transform.position, OnArrived);
                return true;
            }

            return false;
        }

        // TODO: Move to a utility class, and have a registry instead.
        private void FindClosestResourceNode()
        {
            var weapon = _controller.GetAttachment(Attachment.Enum_AttachmentType.Weapon).attachedEquipment as Weapon;
            var resourceSO = weapon.resource;

            FindClosestResourceNode(resourceSO, PrunedAndSortedColliders);
        }

        private void FindClosestResourceNode(ResourceSO resourceSO, List<Collider> colliders)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                var hit = colliders[i];

                var rb = hit.attachedRigidbody;
                
                if (!rb)
                    continue;
                
                if (!rb.TryGetComponent(out ResourceNode resource)) 
                    continue;
                
                if (resource._resourceData != resourceSO)
                    continue;

                target = resource.gameObject;
                _state = Enum_States.Moving;
                _controller.MoveTo(resource.transform.position, OnArrived);
                return;
            }   
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
        
        private void OnArrived()
        {
            if (_state == Enum_States.PickUpAttachment)
            {
                var attachment = _controller.GetAttachment(_targetAttachment);

                if (attachment.HasAttachment)
                {
                    _state = Enum_States.Searching;
                    return;
                }
                
                var success = attachment.TryAttach(target.GetComponent<Equipment>());
                _state = Enum_States.Searching;
                
                return;
            }

            if (_state == Enum_States.PickUpResource)
            {
                Debug.Log("Picking up resource (" + target + ")");
                
                var resource = target.GetComponent<Resource>();
                
                _inventory.TryAddResource(resource);

                if (!_inventory.IsFull)
                    _state = Enum_States.Searching;
                else
                {
                    _state = Enum_States.Returning;
                    
                    Stockpile.stockpiles.TryGetValue(_targetResourceSO, out var stockpile);
                    target = stockpile.gameObject;
                    
                    _controller.MoveTo(stockpile.transform.position, OnArrived);
                    Debug.Log("Returng to stockpile (" + _targetResourceSO.name + ")");
                }

                return;
            }

            if (_state == Enum_States.Returning)
            {
                var stockpile = target.GetComponent<Stockpile>();
                
                _inventory.DepositResources(stockpile);
            }

            _state = Enum_States.Harvesting;
        }
        
        private void State_Moving()
        {
            
        }
        
        private void State_Harvesting()
        {
            var resourceNode = target.GetComponent<ResourceNode>();
            
            if (!resourceNode || !target.activeInHierarchy)
            {
                _state = Enum_States.Searching;
                return;
            }

            _controller.Attack?.Invoke(target);
        }
        
        private void State_Returning()
        {
            
        }

        private void OnDrawGizmos()
        {
            if (!_controller)
                _controller = GetComponent<UnitController>();
            
            Gizmos.DrawWireSphere(transform.position, _controller.vision);
        }
    }
}