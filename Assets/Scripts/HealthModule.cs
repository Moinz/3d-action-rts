using System;
using UnityEngine;

public class HealthModule : MonoBehaviour
{
    private float _health;

    [SerializeField]
    private float _maxHealth;
    
    public float Health => _health;
    public float MaxHealth => _maxHealth;

    public Action OnDeath;
    public Action<float> OnDamaged;

    private void Start()
    {
        _health = MaxHealth;

        gameObject.AddComponent<HealthAnimation>().Initialize(this);
    }
    public void Damage(float amount)
    {
        Debug.Log("Was inflicted damage : " + amount + gameObject.name);
        _health -= amount;
        
        if (_health <= 0)
        {
            OnDeath?.Invoke();
        }
        
        OnDamaged?.Invoke(Health);
    }
    
    public void Heal(float amount)
    {
        _health += amount;
    }

    public void Kill()
    {
        _health = 0;
        OnDeath?.Invoke();
    }
}