using System;
using System.Collections;
using UnityEditor;
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
}

[CustomEditor(typeof(HealthModule))]
public class HealthModuleEditor : Editor
{
    private HealthModule Target;
    private float damage = 100;

    private void OnEnable()
    {
        Target = target as HealthModule;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        damage = EditorGUILayout.FloatField("Damage", damage);

        if (GUILayout.Button("Damage"))
        {
            Target.Damage(damage);
        }
    }
}