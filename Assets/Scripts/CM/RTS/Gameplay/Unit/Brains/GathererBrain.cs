using System;
using System.Collections.Generic;
using UnityEngine;

namespace CM.Units
{
    
    [Serializable]
    public class GathererBrain : Brain
    {
        public GathererBrain(UnitStateController stateController, UnitController unitUnitController)
        {
            _stateController = stateController;
            _unitController = unitUnitController;
        }

        [SerializeField]
        private ResourceSO _targetResourceSO;
        
        [SerializeField]
        private Enum_GatherStates _gatherState;
        
        private enum Enum_GatherStates
        {
            Idle,
            Searching,
            Moving,
            PickUpAttachment,
            PickUpResource,
            Harvesting,
            Returning
        }

        private void State_Moving()
        {
            
        }
        
        private void State_Harvesting()
        {
            var resourceNode = Target.GetComponent<ResourceNode>();
            
            if (!resourceNode || !Target.activeInHierarchy)
            {
                _gatherState = Enum_GatherStates.Searching;
                return;
            }

            _unitController.Attack?.Invoke(Target);
        }
        
        private void State_Returning()
        {
            
        }
        
                
        private void OnArrived()
        {
            if (!Target)
                return;
            
            if (_gatherState == Enum_GatherStates.PickUpAttachment)
            {
                var attachment = _unitController.GetAttachment(_targetAttachment);

                if (attachment.HasAttachment)
                {
                    _gatherState = Enum_GatherStates.Searching;
                    return;
                }
                
                var success = attachment.TryAttach(Target.GetComponent<Equipment>());
                _gatherState = Enum_GatherStates.Searching;
                
                return;
            }

            if (_gatherState == Enum_GatherStates.PickUpResource)
            {
                Debug.Log("Picking up resource (" + Target + ")");
                
                var resource = Target.GetComponent<Resource>();
                
                inventory.TryAddResource(resource);

                if (!inventory.IsFull)
                    _gatherState = Enum_GatherStates.Searching;
                else
                {
                    _gatherState = Enum_GatherStates.Returning;
                    
                    Stockpile.stockpiles.TryGetValue(_targetResourceSO, out var stockpile);
                    Target = stockpile.gameObject;
                    
                    _unitController.MoveTo(stockpile.transform.position, OnArrived);
                    Debug.Log("Returng to stockpile (" + _targetResourceSO.name + ")");
                }

                return;
            }

            if (_gatherState == Enum_GatherStates.Returning)
            {
                var stockpile = _stateController.target.GetComponent<Stockpile>();
                
                inventory.DepositResources(stockpile);
            }

            _gatherState = Enum_GatherStates.Harvesting;
        }
        
                
        public override void Tick()
        {
            switch (_gatherState)
            {
                case Enum_GatherStates.Idle:
                    _gatherState = Enum_GatherStates.Searching;
                    State_Idle();
                    return;
                case Enum_GatherStates.Searching:
                    State_Searching();
                    return;
                case Enum_GatherStates.Moving:
                    State_Moving();
                    return;
                case Enum_GatherStates.Harvesting:
                    State_Harvesting();
                    return;
                case Enum_GatherStates.PickUpAttachment:
                    State_PickUpAttachment();
                    return;
                
                case Enum_GatherStates.Returning:
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
            _stateController.Query_PerformSearch(searchMask);
            
            var weapon = _unitController.GetAttachment(Attachment.Enum_AttachmentType.Weapon).HasAttachment;
            if (!weapon)
            {
                Debug.Log(_stateController.gameObject.name + "No Weapon, finding one.");
                FindAttachment(Attachment.Enum_AttachmentType.Weapon);
                return;
            }

            var shield = _unitController.GetAttachment(Attachment.Enum_AttachmentType.Shield).HasAttachment;
            if (!shield)
            {
                Debug.Log(_stateController.gameObject.name + "No Shield, finding one.");
                FindAttachment(Attachment.Enum_AttachmentType.Shield);
                return;
            }

            if (FindAvailableResource())
                return;
            
            Debug.Log(_stateController.gameObject.name + "Searching for resource");
            FindClosestResourceNode();
        }
        
        private bool FindAvailableResource()
        {
            var weapon = _unitController.GetAttachment(Attachment.Enum_AttachmentType.Weapon).attachedEquipment as Weapon;
            _targetResourceSO = weapon.resource;
            
            Debug.Log("Looking for resource (" + _targetResourceSO.name + ")");
            
            for (int i = 0; i < _stateController.PrunedAndSortedColliders.Count; i++)
            {
                var hit = _stateController.PrunedAndSortedColliders[i];

                var rb = hit.attachedRigidbody;
                
                if (!rb)
                    continue;
                
                if (!rb.TryGetComponent(out Resource resource)) 
                    continue;
                
                if (resource.SO != _targetResourceSO)
                    continue;

                Debug.Log("Found resource (" + _stateController.target + ")");
                
                Target = resource.gameObject;
                _gatherState = Enum_GatherStates.PickUpResource;
                _unitController.MoveTo(resource.transform.position, OnArrived);
                return true;
            }

            return false;
        }
        
        
        private void State_PickUpAttachment()
        {
            Target.TryGetComponent(out Equipment equipment);

            if (!equipment)
                return;

            if (equipment.IsEquipped)
                _gatherState = Enum_GatherStates.Searching;
        }

        // TODO: Move to a utility class, and have a registry instead.
        private void FindClosestResourceNode()
        {
            var weapon = _unitController.GetAttachment(Attachment.Enum_AttachmentType.Weapon).attachedEquipment as Weapon;
            var resourceSO = weapon.resource;

            FindClosestResourceNode(resourceSO, _stateController.PrunedAndSortedColliders);
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
                
                if (resource._resourceSO != resourceSO)
                    continue;
                
                Target = resource.gameObject;
                _gatherState = Enum_GatherStates.Moving;
                _unitController.MoveTo(resource.transform.position, OnArrived);
                return;
            }   
        }
        
        
        private Attachment.Enum_AttachmentType _targetAttachment;
        
        private void FindAttachment(Attachment.Enum_AttachmentType type)
        {
            Debug.Log(_stateController.gameObject.name + " Find Attachment : " + type);
            _targetAttachment = type;
            
            _stateController.Query_PerformSearch(Equipment.Mask);
            
            foreach (var col in _stateController.PrunedAndSortedColliders)
            {
                if (!col.attachedRigidbody)
                    continue;
                
                col.attachedRigidbody.TryGetComponent(out Equipment equipment);
                if (!equipment)
                    return;
                
                if (equipment.attachmentType != type || equipment.IsEquipped)
                    continue;
                
                Debug.Log(_stateController.gameObject.name +" Found Attachment : " + type + " : " + col);
                
                Target = equipment.gameObject;
                _gatherState = Enum_GatherStates.PickUpAttachment;
                _unitController.MoveTo(equipment.transform.position, OnArrived);
                break;
            }
        }

        public override int TickRate => 30;

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            _stateController = stateController;
            _unitController = unitController;
        }
    }
}