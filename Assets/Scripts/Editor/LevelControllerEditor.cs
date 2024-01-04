using System;
using System.Linq;
using CM.WorldGrid;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelController))]
public class LevelControllerEditor : Editor
{
    private LevelController Target;
    
    private void OnEnable()
    {
        Target = target as LevelController;

        if (Target != null)
            Target._waveFunctionCollapse.Initialize(WorldGridEditor._selection, Target._tileData.ToList());
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
            Target.CleanUp();
    }
}