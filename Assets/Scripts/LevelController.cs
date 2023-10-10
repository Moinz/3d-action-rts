using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private ResourceSO[] _resources;

    [SerializeField]
    private Vector2 _levelSize = new(10, 10);

    [SerializeField, Range(0, 1)]
    private float density;

    [SerializeField]
    private int _iterations;

    [SerializeField]
    private float _maxDistance;
    
    private List<GameObject> items = new();

    public void PopulateLevel()
    {
        var treeCount = Mathf.RoundToInt(_levelSize.x * _levelSize.y * density);
        var rockCount = Mathf.RoundToInt(_levelSize.x * _levelSize.y * density);
        
        var itemCount = treeCount + rockCount;
        items = new List<GameObject>(itemCount);

        for (int i = 0; i < itemCount; i++)
        {
            SpawnResource(_resources[i % _resources.Length]);
        }

        EnsureEvenSpread();
    }

    // TODO: Incorporate a Poisson Disc Sampling algorithm
    private void EnsureEvenSpread()
    {
        var iterations = 0;
        var maxIterations = _iterations;
        while (iterations < maxIterations)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                var pos = item.transform.position;

                for (int j = 0; j < items.Count; j++)
                {
                    if (i == j)
                        continue;

                    var otherItem = items[j];

                    var otherPos = otherItem.transform.position;

                    var distance = Vector3.Distance(pos, otherPos);
                    
                    if (distance < _maxDistance)
                    {
                        var inverseDistance = _maxDistance - distance;
                        var direction = (pos - otherPos).normalized;

                        pos += direction * inverseDistance;
                    }
                }

                item.transform.position = pos;
            }
            
            iterations++;
        }
    }

    private void SpawnResource(ResourceSO resourceSO)
    {
        var x = Random.Range(-_levelSize.x / 2, _levelSize.x / 2);
        var z = Random.Range(-_levelSize.y / 2, _levelSize.y / 2);

        var resourceNode = ResourceFactory.CreateResourceNode(resourceSO, new Vector3(x, 0, z), transform);

        resourceNode.name = resourceSO.name + transform.position;
        items.Add(resourceNode.gameObject);
    }

    public void CleanUp()
    {
        var childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            
            DestroyImmediate(child.gameObject);
        }
    }
    
    
}