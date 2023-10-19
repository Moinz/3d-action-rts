using CM.Units;
using UnityEngine;
using Random = UnityEngine.Random;

public class Resource : MonoBehaviour, IInteractable
{
    public ResourceSO SO;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Interact(UnitController unitController)
    {
        unitController._inventory.TryAddResource(this);
    }
}