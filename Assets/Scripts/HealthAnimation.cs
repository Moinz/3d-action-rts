using DG.Tweening;
using UnityEngine;

public class HealthAnimation : MonoBehaviour
{
    public HealthModule healthModule;

    [SerializeField]
    private bool _doColor;
    
    [SerializeField]
    private Color _hurtColor;

    public void Initialize(HealthModule hm)
    {
        healthModule = hm;
        healthModule.OnDamaged += OnDamaged;
    }
    
    private void OnDisable()
    {
        if (!healthModule)
            return;
        
        healthModule.OnDamaged -= OnDamaged;
    }

    private void OnDamaged(float damage)
    {
        var force = transform.localScale * .25f;
        var duration = 0.175f;
        
        ShortcutExtensions.DOPunchScale(transform, force, duration);
        
        if (!_doColor)
            return;
        
        var material = GetComponentInChildren<Renderer>().material;
        material.DOColor(_hurtColor, "_BaseColor", duration).SetLoops(2, LoopType.Yoyo);
    }

    
}