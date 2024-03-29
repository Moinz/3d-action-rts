using CM;
using CM.Units;

public class Resource : EntityBehavior, IInteractable
{
    public ResourceSO SO;

    public void Interact(UnitController unitController)
    {
        unitController._inventory.TryAddResource(this);
    }
}