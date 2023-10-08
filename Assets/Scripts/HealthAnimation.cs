using DG.Tweening;
using UnityEngine;

public class HealthAnimation : MonoBehaviour
{
    public HealthModule healthModule;

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
        var force = transform.localScale * 1.15f;
        var duration = 0.175f;
        
        ShortcutExtensions.DOPunchScale(transform, force, duration);
    }

    
}