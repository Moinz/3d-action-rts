using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class WaveFunctionCollapse
{
    [SerializeField]
    private List<wfc_tile> _grid;
    
    public List<wfc_tile> Grid => _grid;
    
    private int _size;

    private List<TileData> _tileData;
    
    public void Initialize(int size, List<TileData> tileData)
    {
        _size = size;
        _grid = new List<wfc_tile>(size * size);
        _tileData = new List<TileData>(tileData);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                _grid.Add(new wfc_tile(new Vector2Int(x, y), false, _tileData.ToArray()));
            }
        }
    }

    [SerializeField]
    private List<wfc_tile> gridCopy;

    public void Collapse()
    {
        gridCopy = new List<wfc_tile>(_grid);
        gridCopy = gridCopy.Where(x => !x.collapsed).ToList();

        if (gridCopy == null || gridCopy.Count == 0)
            return;
        
        // Sort by length of options
        gridCopy.Sort((a, b) => a.options.Length - b.options.Length);
        
        var length = gridCopy[0].options.Length;
        var stopIndex = 0;

        for (int i = 0; i < gridCopy.Count; i++)
        {
            if (gridCopy[i].options.Length > length)
            {
                stopIndex = i;
                break;
            }
        }
        
        if (stopIndex > 0)
            gridCopy.RemoveRange(stopIndex, gridCopy.Count - stopIndex);

        var bestCandidate = gridCopy[0];
        bestCandidate.collapse();
        gridCopy.RemoveAt(0);

        var nextGrid = new List<wfc_tile>(_grid);
        for (int j = 0; j < _size; j++)
        {
            for (int i = 0; i < _size; i++)
            {
                var index = i + j * _size;
                
                if (index >= nextGrid.Count)
                    continue;
                
                var tile = nextGrid[index];
                if (tile.collapsed)
                    continue;

                var options = new List<TileData>();
                options.AddRange(tile.options); 

                // Up
                if (j > 0)
                {
                    var validOptions = new List<TileData>();
                    var up = _grid[i + (j - 1) * _size];

                    foreach (var t in up.options)
                    {
                        var valid = t.down;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                // Right
                if (i < _size - 1)
                {
                    var validOptions = new List<TileData>();
                    var right = _grid[i + 1 + j * _size];

                    for (int k = 0; k < right.options.Length; k++)
                    {
                        var valid = right.options[k].left;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                // Down
                if (j < _size - 1)
                {
                    var validOptions = new List<TileData>();
                    var down = _grid[i + (j + 1) * _size];

                    for (int k = 0; k < down.options.Length; k++)
                    {
                        var valid = down.options[k].up;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                if (i > 0)
                {
                    var validOptions = new List<TileData>();
                    var left = _grid[i - 1 + j * _size];

                    for (int k = 0; k < left.options.Length; k++)
                    {
                        var valid = left.options[k].right;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                tile.options = options.ToArray();
            }
        }
        
        _grid = nextGrid;
    }
    
    public void checkvalid(ref List<TileData> options, List<TileData> valid)
    {
        for (int i = options.Count - 1; i >= 0;  i--)
        {
            var element = options[i];

            if (!valid.Contains(element))
                options.Remove(element);
        }
    }
    
    [System.Serializable]
    public class wfc_tile
    {
        public Vector2Int position;
        public bool collapsed;
        public TileData[] options;

        public wfc_tile(Vector2Int position, bool collapsed, TileData[] options)
        {
            this.position = position;
            this.collapsed = collapsed;
            this.options = options;
        }

        public void collapse()
        { 
            var pick = Random.Range(0, options.Length);
            var data = options[pick];
            
            Debug.Log($"Picked {pick} from {options.Length} options.");
            
            options = new TileData[1];
            
            options[0] = data;
            collapsed = true;
            
            var go = new GameObject(data.name);
            go.transform.position += new Vector3(position.x, 0f, position.y);
            var tile = go.AddComponent<Tile>();
            var tileGO = GameObject.Instantiate(data.mesh, go.transform);
            
            tileGO.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.AngleAxis(90 * data.rotations, Vector3.up);
            tile._tileData = data;
            go.name = tile.name;
        }
    }
}