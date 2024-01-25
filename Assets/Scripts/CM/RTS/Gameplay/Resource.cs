using CM.Units;

public class Resource : Entity, IInteractable
{
    public ResourceSO SO;

    public void Interact(UnitController unitController)
    {
        unitController._inventory.TryAddResource(this);
        
        if (IsSelected != null)
            IsSelected.Value = false;
    }
}