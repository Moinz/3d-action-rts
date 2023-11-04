using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CM/Tile Data")]
public class TileData : ScriptableObject
{
    public GameObject mesh;
    public int rotations;

    public int[] edges = {0, 0, 0, 0 };
    public bool rotateable = true;

    public float weight = 1f;
    
    public List<TileData> up = new();
    public List<TileData> right = new();
    public List<TileData> down = new ();
    public List<TileData> left = new ();

    public int[] rotate(int rotations)
    {
        this.rotations = rotations;
        var length = edges.Length;
        var newEdges = new int[length];
        
        for (int i = 0; i < length; i++)
        {
            var newIndex = (i + rotations) % length;
            newEdges[newIndex] = edges[i];
        }
        
        return newEdges;
    }
    
    public void analyze(TileData[] tileData)
    {
        up.Clear();
        left.Clear();
        down.Clear();
        right.Clear();
        
        foreach (var data in tileData)
        {
            if (data.edges[2] == edges[0])
                up.Add(data);
            
            if (data.edges[3] == edges[1])
                right.Add(data);    
            
            if (data.edges[0] == edges[2])
                down.Add(data);
            
            if (data.edges[1] == edges[3])
                left.Add(data);
        }
    }

    public static TileData CreateRotatedTileData(TileData data, int i)
    {
        var td = CopyTileData(data);
        td.edges = td.rotate(i);
        td.name += " " + i * 90 + "°";

        return td;
    }
    
    public static TileData CopyTileData(TileData data)
    {
        var td = CreateInstance<TileData>();
        td.name = data.name;
        td.mesh = data.mesh;
        td.edges = data.edges;
        td.rotateable = data.rotateable;
        td.weight = data.weight;

        return td;
    }
}