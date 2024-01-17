using System.Collections.Generic;
using System.Linq;

using CM.WorldGrid;
using UnityEngine;

[System.Serializable]
public class WaveFunctionCollapse
{
    [SerializeField]
    private List<WFCTile> _grid;
    
    public List<WFCTile> Grid => _grid;
    
    private int _size;

    private List<TileData> _tileData;
    
    public static TileWeightTable _tileWeightTable;
    
    public void Initialize(int size, List<TileData> tileData)
    {
        _size = size;
        _grid = new List<WFCTile>(size * size);
        _tileWeightTable = new TileWeightTable();
        _tileData = new List<TileData>(tileData);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                _grid.Add(new WFCTile(new Vector2Int(x, y), false, _tileData.ToArray()));
            }
        }

        foreach (var wfcTile in _grid)
        {
            wfcTile.SetNeighbors(_grid, _size);
        }
    }

    private bool _isInitialized;
    public void Initialize(WorldSelection selection, List<TileData> tileData)
    { 
        if (_isInitialized)
            return;
        
        _isInitialized = true;
        
        _size = selection.size;
        _grid = new List<WFCTile>(selection.size * selection.size);
        _tileWeightTable = new TileWeightTable();
        _tileData = new List<TileData>(tileData);
        var tileDataArray = _tileData.ToArray();

        var cells = selection.Cells;
        foreach (var worldCell in cells)
        {
            _grid.Add(new WFCTile(new Vector2Int(worldCell.position.x, worldCell.position.z), false, tileDataArray));
        }
        
        foreach (var wfcTile in _grid)
        {
            wfcTile.SetNeighbors(_grid, _size);
        }
    }

    [SerializeField]
    private List<WFCTile> gridCopy;

    public void Collapse()
    {
        gridCopy = new List<WFCTile>(_grid);
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

        var nextGrid = new List<WFCTile>(_grid);
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                var index = x + y * _size;
                
                if (index >= nextGrid.Count)
                    continue;
                
                var tile = nextGrid[index];
                if (tile.collapsed)
                    continue;

                var options = new List<TileData>();
                options.AddRange(tile.options); 

                // Up
                if (tile.up != null)
                {
                    var validOptions = new List<TileData>();
                    var up = tile.up;

                    foreach (var t in up.options)
                    {
                        var valid = t.down;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                // Right
                if (tile.right != null)
                {
                    var validOptions = new List<TileData>();
                    var right = tile.right;

                    for (int k = 0; k < right.options.Length; k++)
                    {
                        var valid = right.options[k].left;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                // Down
                if (tile.down != null)
                {
                    var validOptions = new List<TileData>();
                    var down = tile.down;

                    for (int k = 0; k < down.options.Length; k++)
                    {
                        var valid = down.options[k].up;
                        validOptions.AddRange(valid);
                    }
                    
                    checkvalid(ref options, validOptions);
                }
                
                if (tile.left != null)
                {
                    var validOptions = new List<TileData>();
                    var left = tile.left;

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
    
    
}