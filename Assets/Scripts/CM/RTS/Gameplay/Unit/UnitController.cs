using System;
using CM.RTS.Gameplay;
using UnityEngine;

namespace CM.Units
{
    public partial class UnitController : MonoBehaviour
    {
        private UnitLocomotion _unitLocomotion;

        [SerializeField] 
        private Attachment[] _attachments;

        private HealthModule _healthModule;
        internal Inventory _inventory;
        public Action<GameObject> Attack;
        
        public void Initialize(UnitArchetype archetype)
        {
            _archetype = archetype;
            team = archetype.team;
            
            _healthModule = GetComponent<HealthModule>();

            _inventory = GetComponent<Inventory>();
            _attachments = GetComponentsInChildren<Attachment>();
            
            _statistics = new UnitStatistics(_archetype.statistics);
            _unitLocomotion = GetComponent<UnitLocomotion>();
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
            if (_inventory)
                _inventory.EjectInventory();

            gameObject.SetActive(false);
        }
    }
    
    public partial class UnitController
    {
        public UnitArchetype _archetype;
        public UnitStatistics _statistics;
        
        // Statistics
        public float interactRange => GlobalUnitSettings.Instance.interactRange;
        public float vision => _statistics.vision;
        
        // Combat Statistics
        public float attack => _statistics.attack;
        public float attackSpeed => _statistics.attackSpeed;
        public float defense => _statistics.defense;
        
        // Attributes
        public float strength => _statistics.strength;
        public float agility => _statistics.agility;
        public float intelligence => _statistics.intelligence;

        public enum Enum_Team
        {
            Player,
            Bandits,
            Fauna
        }
        
        public Enum_Team team;

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
            _unitLocomotion.Stop();
        }
        
        public void MoveTo(Vector3 position, Action onArrived = null)
        {
            _unitLocomotion.MoveTo(position, onArrived);
        }
        
        public bool IsMoving()
        {
            return _unitLocomotion.IsMoving();
        }
    }
}