using CM.Units;
using TriInspector;
using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(fileName = "UnitArchetype", menuName = "CM/Units/Archetype")]
    public class UnitArchetype : ScriptableObject
    {
        public string name;
        public UnitController.Enum_Team team;
        
        [AssetsOnly]
        public GameObject prefab;
        
        [AssetsOnly]
        public GameObject visuals;
        
        [AssetsOnly]
        public BrainData brainData;

        public Equipment[] startingEquipment;
        
        public UnitStatistics statistics;


    }
}

public static class UnitFactory
{
    public static UnitStateController CreateUnit(UnitArchetype archetype, Vector3 position)
    {
        // Create Instance
        var instance = Object.Instantiate(archetype.prefab, position, Quaternion.identity);
        instance.name = archetype.name;
        
        // Initialize Unit
        var unitController = instance.GetComponent<UnitStateController>();
        unitController.Initialize(archetype);
        
        // Set up visuals
        var visuals = Object.Instantiate(archetype.visuals, instance.transform);
        visuals.transform.localPosition = Vector3.zero;

        // Attach equipment
        var attachments = visuals.GetComponentsInChildren<Attachment>();
        foreach (var equipment in archetype.startingEquipment)
        {
            var equipmentInstance = Object.Instantiate(equipment, instance.transform);
            
            foreach (var attachment in attachments)
            {
                if (attachment.attachmentType != equipment.attachmentType)
                    continue;
                
                if (attachment.HasAttachment)
                    continue;
                
                if (equipmentInstance.TryAttach(attachment))
                    break;
            }
        }
        
        // Set up Interactions
        InteractionController.SetUpInteractableTriggerCollider(instance.gameObject);
        
        return unitController;
    }
}