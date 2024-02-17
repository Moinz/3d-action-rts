using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CM.RTS.Gameplay
{
    public class HealthModule : MonoBehaviour
    {
        public ArmorModule ArmorModule = new();
        
        private float _health;

        [SerializeField]
        private float _maxHealth;
    
        public float Health => _health;
        public float MaxHealth => _maxHealth;

        public Action OnDeath;
        public Action<float> OnHealthChanged;
        
        public float HealthPointsPerOrb = 30;

        public int MaxHealthOrbs => Mathf.CeilToInt(MaxHealth / HealthPointsPerOrb);
        public int CurrentOrbs => Mathf.CeilToInt(Health / HealthPointsPerOrb);

        private void Awake()
        {
            _health = MaxHealth;
            gameObject.AddComponent<HealthAnimation>().Initialize(this);
            
            ArmorModule.Init(MaxHealthOrbs, new Vector2Int(0, 3));
        }
    
        public void Damage(float amount)
        {
            Debug.Log(gameObject.name + " was inflicted damage : " + amount, gameObject);
            
            ArmorModule.CurrentOrbIndex = CurrentOrbs - 1;
            var remaining = ArmorModule.AbsorbDamage(amount);
            
            Debug.Log(gameObject.name + " went through armor : " + remaining, gameObject);
            _health -= remaining;
        
            if (_health <= 0)
            {
                _health = MaxHealth;
                ArmorModule.Init(CurrentOrbs, new Vector2Int(0, 3));
            }
        
            OnHealthChanged?.Invoke(Health);
        }
    
        public void Heal(float amount)
        {
            Debug.Log(gameObject.name + " was healed: " + amount, gameObject);
            _health += amount;
        
            if (_health > MaxHealth)
                _health = MaxHealth;
        
            OnHealthChanged?.Invoke(Health);
        }

        public void Kill()
        {
            Damage(Health);
        }
    }
    
    public class ArmorModule
    {
        public int CurrentOrbIndex;
        private int ArmorPerShard = 25;
        
        private Vector2Int ArmorShardRange = new(0, 3);
        private List<int> CurrentArmorValues;

        public int CurrentArmor => CurrentArmorValues[CurrentOrbIndex];
        public Action<float> OnArmorChanged;

        public int ArmorShardsAtIndex(int index)
        {
            if (CurrentArmorValues == null || CurrentArmorValues.Count == 0)
                return 0;
            
            if (index >= CurrentArmorValues.Count)
                return 0;

            var value = CurrentArmorValues[index];
            if (value == 0)
                return 0;
            
            return value / ArmorPerShard;
        }

        public void Init(int orbs, Vector2Int range)
        {
            ArmorShardRange = range;
            
            ConstructArmorShards(orbs);
        }

        private void ConstructArmorShards(int orbs)
        {
            var maxOrbs = orbs;
            CurrentArmorValues = new List<int>(maxOrbs);
            
            for (int i = 0; i < maxOrbs; i++)
            {
                var armorShards = UnityEngine.Random.Range(ArmorShardRange.x, ArmorShardRange.y);
                var armorValue = armorShards * ArmorPerShard;
                
                
                CurrentArmorValues.Add(armorValue);
            }
        }
        
        public float AbsorbDamage(float amount)
        {
            if (CurrentArmorValues == null || CurrentArmorValues.Count == 0)
                return amount;

            var armor = CurrentArmor;
            var remaining = armor - amount;
            
            if (remaining <= 0)
            {
                CurrentArmorValues[CurrentOrbIndex] = 0;
                OnArmorChanged?.Invoke(0);
                return Mathf.Abs(remaining);
            }
            
            CurrentArmorValues[CurrentOrbIndex] = (int) remaining;
            OnArmorChanged?.Invoke(remaining);
            return 0;
        }
    }
}