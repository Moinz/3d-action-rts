using System;
using CM.Units;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Definition", menuName = "CM/Crafting/Crafting Definition", order = 1)]
public class CraftingDefinition : ScriptableObject
{
    public Requirement[] requirements;
    public GameObject result;
    
    [Serializable]
    public struct Requirement
    {
        public ResourceSO resource;
        public int amount;
    }

    public bool MeetsRequirement(Inventory inventory)
    {
        foreach (var requirement in requirements)
        {
            if (inventory.Count(requirement.resource) < requirement.amount)
                return false;
        }
        
        return true;
    }
    
    public bool ConsumeRequirements(Inventory inventory)
    {
        if (!MeetsRequirement(inventory))
            return false;
        
        foreach (var requirement in requirements)
        {
            for (int i = 0; i < requirement.amount; i++)
            {
                var resource = inventory.GetResource(requirement.resource);

                if (resource == null)
                    throw new Exception("Resource not found in inventory");
                
                inventory.TryRemoveResource(resource);
                Destroy(resource.gameObject);
            }
        }

        return true;
    }
    
}