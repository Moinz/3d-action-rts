using System;
using UnityEditor;
using UnityEngine;

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
        
        if (GUILayout.Button("Generate"))
            Target.GenerateLevel();
        
        if (GUILayout.Button("Spawn Tiles"))
            Target.SpawnTileData();

        if (GUILayout.Button("Generate Tile Data"))
            Target.GenerateTileData();
        
        if (GUILayout.Button("Collapse"))
            Target._waveFunctionCollapse.Collapse();

        if (GUILayout.Button("Clean Up"))
        {
            Target.CleanUp();
        }
    }

    private void OnSceneGUI()
    {
        foreach (var tile in Target._waveFunctionCollapse.Grid)
        {
            if (tile.collapsed)
                continue;
            
            var tilePos = tile.position;
            var pos = Target.transform.position + new Vector3(tilePos.x, 0f, tilePos.y);

            Handles.color = Color.red;
            Handles.DrawWireCube(pos, Vector3.one);
            Handles.Label(pos, tile.options.Length.ToString());
            
            
        }
    }
}