using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    [SerializeField] 
    private int _levelSize = 10;

    [SerializeField, Range(0, 1)]
    private float density;

    [SerializeField]
    private int _iterations;

    [SerializeField]
    private float _maxDistance;
    
    private List<GameObject> items = new();

    [SerializeField]
    public WaveFunctionCollapse _waveFunctionCollapse;
    
    private bool automatic;

    public static LevelController Instance => _instance ? _instance : _instance = FindObjectOfType<LevelController>();
    private static LevelController _instance;

    private void Awake()
    {
        //GenerateLevel();
    }
    
    [SerializeField]
    public TileData[] _tileData;

    public void GenerateLevel()
    {
        CleanUp();
        
        _waveFunctionCollapse = new WaveFunctionCollapse();
        
        if (_tileData == null || _tileData.Length == 0)
            GenerateTileData();

        _waveFunctionCollapse.Initialize(_levelSize, _tileData.ToList());
        _waveFunctionCollapse.Collapse();
    }

    public void GenerateTileData()
    {
        _tileData = null;
        
        var tiles = Resources.LoadAll<TileData>("tiledata");
        var tileData = new List<TileData>();
        
        foreach (var data in tiles)
        {
            tileData.Add(TileData.CopyTileData(data));
            
            if (!data.rotateable)
                continue;
            
            tileData.Add(TileData.CreateRotatedTileData(data, 1));
            tileData.Add(TileData.CreateRotatedTileData(data, 2));
            tileData.Add(TileData.CreateRotatedTileData(data, 3));
        }

        var tileDataArray = tileData.ToArray();
        foreach (var data in tileData)
        {
            data.analyze(tileDataArray);
        }

        _tileData = tileData.ToArray();
    }

    public void SpawnTileData()
    {
        if (_tileData == null || _tileData.Length == 0)
            GenerateTileData();
        
        for (var index = 0; index < _tileData.Length; index++)
        {
            var data = _tileData[index];
            var go = new GameObject(data.name);
            go.transform.position += new Vector3(index * 1.25f, 0, -10);
            go.transform.SetParent(transform);
            
            var tile = go.AddComponent<Tile>();
            var tileGO = Instantiate(data.mesh, go.transform);

            tileGO.transform.localPosition = Vector3.zero;
            tileGO.transform.localRotation = Quaternion.AngleAxis(90 * data.rotations, Vector3.up);
            tile._tileData = data;
        }
    }

    private void Update()
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            automatic = !automatic;
        
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            GenerateLevel();
            return;
        }
        
        if (automatic)
        {
            _waveFunctionCollapse.Collapse();
            return;
        }
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _waveFunctionCollapse.Collapse();
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