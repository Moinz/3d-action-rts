using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthModule))]
public class HealthModuleEditor : Editor
{
    private HealthModule Target;
    private float damage = 100;

    private void OnEnable()
    {
        Target = target as HealthModule;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        damage = EditorGUILayout.FloatField("Damage", damage);

        if (GUILayout.Button("Damage"))
        {
            Target.Damage(damage);
        }
    }
}