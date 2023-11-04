using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    private Tile Target;
    
    private void OnEnable()
    {
        Target = target as Tile;
    }

    private void OnSceneGUI()
    {
        var transform = Target.transform;
        var pos = transform.position;
        
        Handles.color = Color.blue;
        Handles.Label(pos + transform.forward, Target._tileData.edges[0].ToString());
        Handles.Label(pos + transform.right, Target._tileData.edges[1].ToString());
        Handles.Label(pos + -transform.forward, Target._tileData.edges[2].ToString());
        Handles.Label(pos + -transform.right, Target._tileData.edges[3].ToString());
    }
}