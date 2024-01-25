using CM.Units;
using UnityEngine;

public interface IInteractable
{
    public GameObject gameObject { get; }
    public void Interact(UnitController unitController);
}