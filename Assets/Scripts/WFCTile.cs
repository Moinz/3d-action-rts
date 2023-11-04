using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WFCTile
{
    public Vector2Int position;
    public bool collapsed;
    public TileData[] options;
        
    [SerializeField]
    public WFCTile up;

    public Vector2Int upPosition;
    [SerializeField]
    public WFCTile right;
    public Vector2Int rightPosition;
    [SerializeField]
    public WFCTile down;
    public Vector2Int downPosition;
    [SerializeField]
    public WFCTile left;
    public Vector2Int leftPosition;

    public WFCTile(Vector2Int position, bool collapsed, TileData[] options)
    {
        this.position = position;
        this.collapsed = collapsed;
        this.options = options;
    }

    public void collapse()
    {
        WaveFunctionCollapse._tileWeightTable.SetTileData(options);
        var data = WaveFunctionCollapse._tileWeightTable.GetRandom();
            
        Debug.Log($"Picked {data} from {options.Length} options.");
            
        options = new TileData[1];
            
        options[0] = data;
        collapsed = true;
            
        var go = new GameObject(data.name);
        go.transform.SetParent(LevelController.Instance.transform);
        go.transform.position += new Vector3(position.x, 0f, position.y);
        var tile = go.AddComponent<Tile>();
        var tileGO = GameObject.Instantiate(data.mesh, go.transform);
            
        tileGO.transform.localPosition = Vector3.zero;
        tileGO.transform.localRotation = Quaternion.AngleAxis(90 * data.rotations, Vector3.up);
        tile.Initialize(this);
        go.name = tile.name;
    }

    public void SetNeighbors(List<WFCTile> grid, int size)
    {
        up = getNeighbor(grid, position + Vector2Int.up, size);
        upPosition = position + Vector2Int.up;
        right = getNeighbor(grid, position + Vector2Int.right, size);
        rightPosition = position + Vector2Int.right;
        down = getNeighbor(grid, position + Vector2Int.down, size);
        downPosition = position + Vector2Int.down;
        left = getNeighbor(grid, position + Vector2Int.left, size);
        leftPosition = position + Vector2Int.left;
    }
        
    private WFCTile getNeighbor(List<WFCTile> grid, Vector2Int position, int size)
    {
        if (position.x < 0 || position.x >= size || position.y < 0 || position.y >= size)
            return null;
            
        return getTile(grid, position);
    }

    private WFCTile getTile(List<WFCTile> grid, Vector2Int position)
    {
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].position == position)
                return grid[i];
        }

        return null;
    }
}