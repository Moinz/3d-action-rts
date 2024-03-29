using CM.RTS.Gameplay;
using UnityEngine;

public class Weapon : Equipment
{
    public float damage;
    public float attackSpeed;

    public ResourceSO resource;

    private bool _isOnCooldown;

    public override void Use(GameObject target)
    {
        Attack(target);
    }
    
    private void Attack(GameObject target)
    {
        if (_isOnCooldown)
            return;
        
        Debug.Log($"Using {gameObject.name} against {target.name}");

        var healthModule = target.GetComponent<HealthModule>();
        if (!healthModule)
        {
            Debug.Log($"Target has no healthmodule", target);
            return;
        }
        
        healthModule.Damage(damage);
        
        _isOnCooldown = true;
        Invoke(nameof(ResetCooldown), attackSpeed);
    }

    private void ResetCooldown()
    {
        _isOnCooldown = false;
    }
}