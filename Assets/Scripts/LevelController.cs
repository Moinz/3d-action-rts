using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    private LevelController Target;
    private void OnEnable()
    {
        Target = target as LevelController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Populate Level"))
        {
            Target.PopulateLevel();
        }

        if (GUILayout.Button("Clean Up"))
        {
            Target.CleanUp();
        }
    }
}
public class LevelController : MonoBehaviour
{
    [SerializeField]
    private GameObject _tree;

    [SerializeField]
    private GameObject _rock;
    
    [SerializeField]
    private Vector2 _levelSize = new(10, 10);

    [SerializeField, Range(0, 1)]
    private float density;

    [SerializeField]
    private int _iterations;

    [SerializeField]
    private float _maxDistance;

    public void PopulateLevel()
    {
        var treeCount = Mathf.RoundToInt(_levelSize.x * _levelSize.y * density);
        var rockCount = Mathf.RoundToInt(_levelSize.x * _levelSize.y * density);
        
        var items = new List<GameObject>(treeCount + rockCount);
        
        for (int i = 0; i < treeCount; i++)
        {
            var x = Random.Range(-_levelSize.x / 2, _levelSize.x / 2);
            var z = Random.Range(-_levelSize.y / 2, _levelSize.y / 2);
            
            var tree = PrefabUtility.InstantiatePrefab(_tree, transform) as GameObject;
            tree.transform.position = new Vector3(x, 0f, z);
            tree.name = "Tree #" + i;
            items.Add(tree);
        }
        
        for (int i = 0; i < rockCount; i++)
        {
            var x = Random.Range(-_levelSize.x / 2, _levelSize.x / 2);
            var z = Random.Range(-_levelSize.y / 2, _levelSize.y / 2);
            
            var rock = PrefabUtility.InstantiatePrefab(_rock, transform) as GameObject;
            rock.transform.position = new Vector3(x, 0f, z);
            rock.name = "Rock #" + i;
            items.Add(rock);
        }

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