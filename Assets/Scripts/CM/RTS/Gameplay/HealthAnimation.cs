using CM.RTS.Gameplay;
using DG.Tweening;
using UnityEngine;

public class HealthAnimation : MonoBehaviour
{
    public HealthModule healthModule;

    [SerializeField]
    private bool _doColor;
    
    [SerializeField]
    private Color _hurtColor;

    private Vector2 _minForce = Vector3.one * 0.01f;
    private Vector2 _maxForce = Vector3.one * 0.15f;

    public void Initialize(HealthModule hm)
    {
        healthModule = hm;
        healthModule.OnHealthChanged += OnDamaged;
    }
    
    private void OnDisable()
    {
        if (!healthModule)
            return;
        
        healthModule.OnHealthChanged -= OnDamaged;
    }

    private void OnDamaged(float damage)
    {
        var damagePercentage = damage / healthModule.MaxHealth;
        var force = Vector3.Lerp(_minForce, _maxForce, damagePercentage);
        var duration = 0.175f;
        
        ShortcutExtensions.DOPunchScale(transform, force, duration);
        
        if (!_doColor)
            return;
        
        var material = GetComponentInChildren<Renderer>().material;
        material.DOColor(_hurtColor, "_BaseColor", duration).SetLoops(2, LoopType.Yoyo);
    }

    
}