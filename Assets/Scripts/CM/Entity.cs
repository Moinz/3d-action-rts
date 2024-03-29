using System;
using CM.RTS.Gameplay;
using CM.Units;
using UnityEngine;

namespace CM
{
    [RequireComponent(typeof(Rigidbody), typeof(HealthModule))]
    public class Entity : MonoBehaviour, ISelectable, IInteractable
    {
        // Status
        [SerializeField]
        private Observable<EntityStatus> _status = new(EntityStatus.Pooled);
        public Observable<EntityStatus> Status => _status ??= new Observable<EntityStatus>(EntityStatus.Pooled);
        

        public Observable<bool> IsSelected { get; set; }

        public void SetSelected(bool isSelected)
        {
            IsSelected.Value = isSelected;
        }
        
        public bool RegisterSelectionCallback(Action<bool> callback)
        {
            IsSelected.OnValueChanged += callback;
            return true;
        }
        
        public bool UnregisterSelectionCallback(Action<bool> callback)
        {
            IsSelected.OnValueChanged -= callback;
            return true;
        }

        // Core Components
        private Rigidbody _rigidbody;
        private HealthModule _healthModule;

        private void Init()
        {
            _healthModule = GetComponent<HealthModule>();
            _rigidbody = GetComponent<Rigidbody>();
            
            IsSelected = new Observable<bool>(false);
            
            var entityBehaviors = GetComponentsInChildren<EntityBehavior>();
            
            foreach (var behavior in entityBehaviors)
                behavior.Init(this);
            
            _healthModule.OnDeath += () => SetStatus(EntityStatus.Dead);
            
            SetStatus(EntityStatus.Init);
            SetStatus(EntityStatus.Alive);
        }

        private void Pool()
        {
            _healthModule.OnDeath -= () => SetStatus(EntityStatus.Dead);
            
            SetStatus(EntityStatus.Pooled);
        }

        public void OnEnable()
        {
            Init();
            
            
        }

        public void OnDisable()
        {
            Pool();
        }


        Rigidbody ISelectable.Rigidbody
        {
            get => _rigidbody;
            set => _rigidbody = value;
        }
        
        public void SetStatus(EntityStatus newStatus)
        {
            Status.Value = newStatus;
        }

        public void Interact(UnitController unitController)
        {
        }
    }

    public class EntityBehavior : MonoBehaviour
    {
        private Entity _connectedEntity;

        protected Entity ConnectedEntity => _connectedEntity;

        public void Init(Entity entity)
        {
            _connectedEntity = entity;
        }
    }

    public enum EntityStatus
    {
        Init,
        Alive,
        Dead,
        Pooled
    }
}