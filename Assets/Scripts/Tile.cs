using System;
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

public class Tile : MonoBehaviour
{
    public TileData _tileData;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 2f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2f);
    }
}