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