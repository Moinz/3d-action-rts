using System;
using CM.RTS.Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

namespace CM.Units.RTS.UI
{
    [CustomEditor(typeof(HealthBarUI))]
    public class HealthBarUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Construct Healthbar Hierarchy"))
            {
                var healthBarUI = (HealthBarUI) target;
                healthBarUI.ConstructHealthbarHierarchy();
            }
        }
    }
    
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
    
    public class HealthBarUI : MonoBehaviour
    {
        public UIDocument UIDocument;
        public HealthModule HealthModule;

        private VisualElement _healthBar;
        private bool _initialized;
        private Camera _camera;

        #region Initialization

        private void Start()
        {   
            _camera = Camera.main;
        }

        private void Init()
        {
            _initialized = true;
            
            UIDocument.rootVisualElement.Clear();
            
            _healthBar = new HealthBarElement(OnHealthBarClicked);
            UIDocument.rootVisualElement.Add(_healthBar);
       }
        
        public void ConstructHealthbarHierarchy()
        {
            Init();
            
            ConstructHealthOrbs();
            ConstructArmorShards();
        }
        
        private void ConstructHealthOrbs()
        {
            var health = HealthModule.MaxHealthOrbs;
            
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
            HealthModule.OnHealthChanged += UpdateHealthContainer;
            HealthModule.ArmorModule.OnArmorChanged += UpdateHealthContainer;
            
            ConstructHealthbarHierarchy();
        }

        private void OnDisable()
        {
            HealthModule.OnHealthChanged -= UpdateHealthContainer;
            HealthModule.ArmorModule.OnArmorChanged -= UpdateHealthContainer;
        }
        
        private void Update()
        {
            if (!_initialized)
                return;

            var root = UIDocument.rootVisualElement;
            var worldPos = HealthModule.transform.position;
            
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                root.panel, worldPos, _camera);
            
            newPosition.x -= root.layout.width / 2;
            newPosition.y -= root.layout.height / 2;
            
            root.transform.position = newPosition;
        }
        
        #endregion

        
        
        private void OnHealthBarClicked()
        {
            HealthModule.Damage(50);
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