using CM.Units;
using Shapes;
using TMPro;
using UnityEngine;

public class CraftingStation : Entity, IInteractable
{
    [SerializeField]
    private TextMeshPro _textMeshPro;

    [SerializeField]
    private RegularPolygon _regularPolygon;

    [SerializeField]
    private CraftingDefinition _craftingDefinition;

    private void OnEnable()
    {
        _textMeshPro.text = $"Crafting Station \n{_craftingDefinition.result.gameObject.name}";
        meetsRequirement = new Observable<bool>(false);

        uc = FindObjectOfType<UnitController>();
        
        meetsRequirement.OnValueChanged += (meetsRequirement) =>
        {
            _regularPolygon.Color = meetsRequirement ? Color.green : Color.red;
        };
        
        meetsRequirement.Value = _craftingDefinition.MeetsRequirement(uc._inventory);
    }

    private UnitController uc;
    private Observable<bool> meetsRequirement;
    
    private void Update()
    {
        if (Time.frameCount % 30 != 0)
            return;

        meetsRequirement.Value = _craftingDefinition.MeetsRequirement(uc._inventory);
    }

    public void Interact(UnitController unitController)
    {
        var inventory = unitController._inventory;

        if (!meetsRequirement.Value)
            return;
        
        _craftingDefinition.ConsumeRequirements(inventory);
        
        var spawnPos = transform.position;
        spawnPos.y += 0.5f;
        
        var result = Instantiate(_craftingDefinition.result, spawnPos, Quaternion.identity);
        ResourceFactory.Throw(result, RandomPointInCircle(spawnPos, 2.5f));

        InteractionController.SetUpInteractableTriggerCollider(result);
    }
    
    // TODO: Move this to a utility class.
    private Vector3 RandomPointInCircle(Vector3 center, float radius)
    {
        var point = Random.insideUnitCircle * radius;
        
        return center + new Vector3(point.x, 0f, point.y);
    }
}