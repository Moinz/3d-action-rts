using System;
using CM.RTS.Gameplay;
    
using UnityEngine;
using UnityEngine.UIElements;

namespace CM.Units.RTS.UI
{
    public class HealthBarElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<HealthBarElement, UxmlTraits> { }

        public HealthBarElement()
        {
            AddToClassList("HealthBar");
        }
        
        public HealthBarElement(Action onHealthBarClicked)
        {
            AddToClassList("HealthBar");
            RegisterCallback<MouseDownEvent>(e => onHealthBarClicked());
        }
    }
    
    public class HealthBarUI : EntityBehavior
    {
        public UIDocument UIDocument;
        public HealthModule HealthModule;

        private VisualElement _healthBar;
        private bool _initialized;
        private Camera _camera;

        #region Initialization
        
        private void Init()
        {
            _initialized = true;
            
            HealthModule.OnHealthChanged += UpdateHealthContainer;
            HealthModule.ArmorModule.OnArmorChanged += UpdateHealthContainer;
            
            _camera = Camera.main;
            UIDocument.rootVisualElement.Clear();
            
            _healthBar = new HealthBarElement(OnHealthBarClicked);
            UIDocument.rootVisualElement.Add(_healthBar);
            
            ConstructHealthOrbs();
            ConstructArmorShards();
       }
        
        private void Pool()
        {
            _initialized = false;
            
            HealthModule.OnHealthChanged -= UpdateHealthContainer;
            HealthModule.ArmorModule.OnArmorChanged -= UpdateHealthContainer;
            
            UIDocument.rootVisualElement.Clear();
        }
        
        private void ConstructHealthOrbs()
        {
            var health = HealthModule.CurrentOrbs;
            
            for (int i = 0; i < health; i++)
            {
                var healthContainer = CreateVE("HealthContainer", new[] { "HealthContainer" });

                var healthOrb = CreateVE("HealthOrb", new[] { "Orb" });
                
                _healthBar.Add(healthContainer);
                healthContainer.Add(healthOrb);
            }
        }

        private void ConstructArmorShards()
        {
            for (int i = 0; i < _healthBar.childCount; i++)
            {
                var element = _healthBar[i];
                var shardAmount = HealthModule.ArmorModule.ArmorShardsAtIndex(i);

                for (int j = 0; j < shardAmount; j++)
                {
                    var armorShard = CreateVE("ArmorShard", new[] { "Shard" });
                    
                    element.Add(armorShard);
                }
            }
        }

        #endregion


        #region Unity Messages
        
        private void OnEnable()
        {
            ConnectedEntity.RegisterSelectionCallback(OnSelected);   
         
            OnSelected(false);
        }

        private void OnDisable()
        {
            ConnectedEntity.UnregisterSelectionCallback(OnSelected);
        }

        private void OnSelected(bool obj)
        {
            if (obj)
                Init();
            else
                Pool();
        }

        private void Update()
        {
            if (!_initialized)
                return;

            var root = UIDocument.rootVisualElement;

            Position(root);
            Scale(root);
        }

        private void Scale(VisualElement element)
        {
            var scaleFactor = (1 - CameraController.CurrentScale);
            scaleFactor = CameraController.map(scaleFactor, 0f, 1f, 0.6f, 1f);
            
            var scale = Vector3.one * scaleFactor;
            element.style.scale = new StyleScale(new Scale(scale));
        }

        private void Position(VisualElement element)
        {
            var worldPos = HealthModule.transform.position;
            worldPos += Camera.main.transform.up * 1f;
            
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                element.panel, worldPos, _camera);
            
            newPosition.x -= element.layout.width / 2;
            newPosition.y -= element.layout.height / 2;
            
            element.transform.position = newPosition;
        }
        
        
        #endregion
        
        private void OnHealthBarClicked()
        {
            // Something clever here.
        }
        
        private void UpdateHealthContainer(float obj = 0)
        {
            var currentOrbAmount = HealthModule.CurrentOrbs;
            
            var containers = _healthBar.Query<VisualElement>("HealthContainer").ToList();

            for (int i = containers.Count - 1; i >= 0; i--)
            {
                var container = containers[i];

                var healthOrb = container.Q<VisualElement>("HealthOrb");
                var healthOrbInactive = i >= currentOrbAmount;
                UpdateInActivityClass(healthOrb, healthOrbInactive);
                
                var armorShards = container.Query<VisualElement>("ArmorShard").ToList();
                for (var index = 0; index < armorShards.Count; index++)
                {
                    var element = armorShards[index];
                    
                    var armorShardsAtIndex = HealthModule.ArmorModule.ArmorShardsAtIndex(i);
                    var active = healthOrbInactive || index >= armorShardsAtIndex;

                    UpdateInActivityClass(element, active);
                }
            }
        }
        
        private void UpdateInActivityClass(VisualElement element, bool value)
        {
            var healthContainer = element;
            SetInactivityClass(healthContainer, value);
        }

        private void SetInactivityClass(VisualElement element, bool value)
        {
            if (value && !element.ClassListContains("Inactive"))
                element.AddToClassList("Inactive");
                
            if (!value && element.ClassListContains("Inactive"))
                element.RemoveFromClassList("Inactive");
        }
        
        private VisualElement CreateVE(string name, string[] classes)
        {
            var ve = CreateVE(name);

            foreach (var s in classes)
            {
                ve.AddToClassList(s);
            }
            
            return ve;
        }
        
        private VisualElement CreateVE(string name)
        {
            var ve = new VisualElement { name = name};
            
            return ve;
        }
        
    }
}