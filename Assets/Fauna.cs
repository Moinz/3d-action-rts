using CM.Units;
using UnityEngine;

public class Fauna : MonoBehaviour, IInteractable
{
    public void Interact(UnitController unitController)
    {
        unitController.Attack?.Invoke(gameObject);
    }
}