using CM.Units;
using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(fileName = "UnitArchetype", menuName = "CM/Units/Archetype")]
    public class UnitArchetype : ScriptableObject
    {
        public GameObject prefab;
        public BrainData brainData;
        
        public string name;
        public UnitStatistics statistics;
    }
    
}

public static class UnitFactory
{
    public static UnitStateController CreateUnit(UnitArchetype archetype, Vector3 position)
    {
        var instance = Object.Instantiate(archetype.prefab, position, Quaternion.identity);
        var unitController = instance.GetComponent<UnitStateController>();
        unitController.Initialize(archetype);
        
        InteractionController.SetUpInteractableTriggerCollider(instance.gameObject);
        
        return unitController;
    }
}