using CM.Units;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(HealthModule))]
public class ResourceNode : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Vector2Int _resourceRange = new (1,3);

    [FormerlySerializedAs("_resourceData")] [SerializeField]
    public ResourceSO _resourceSO;
    
    private int _resourceCount; 
    public int ResourceCount => _resourceCount;

    [SerializeField]
    private HealthModule _healthModule;

    private void Start()
    {
        _resourceCount = Random.Range(_resourceRange.x, _resourceRange.y);
    }
    
    private void OnEnable()
    {
        _healthModule.OnDeath += OnDeath;    
    }

    private void OnDisable()
    {
        _healthModule.OnDeath -= OnDeath;
    }

    private void OnDeath()
    {
        var pos = transform.position;
        
        gameObject.SetActive(false);
        var spawnPos = transform.position;
        spawnPos.y += 0.5f;
        
        for (int i = 0; i < ResourceCount; i++)
        {
            var resource = ResourceFactory.CreateResource(_resourceSO, spawnPos);
            
            resource.Throw(RandomPointInCircle(pos, 2.5f));
        }
    }
    
    private Vector3 RandomPointInCircle(Vector3 center, float radius)
    {
        var point = Random.insideUnitCircle * radius;
        
        return center + new Vector3(point.x, 0f, point.y);
    }

    public void Interact(UnitController unitController)
    {
        _healthModule.Kill();
    }
}