using UnityEditor;
using UnityEngine;

namespace CM.Units
{
    [CustomEditor(typeof(UnitController))]
    public class UnitControllerEditor : Editor
    {
        private UnitController Target => target as UnitController;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var unitController = Target;

            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Interact Range", unitController.interactRange.ToString());
            EditorGUILayout.LabelField("Vision", unitController.vision.ToString());
            
            EditorGUILayout.LabelField("Attack", unitController.attack.ToString());
            EditorGUILayout.LabelField("Attack Speed", unitController.attackSpeed.ToString());
            EditorGUILayout.LabelField("Defense", unitController.defense.ToString());
            
            EditorGUILayout.LabelField("Strength", unitController.strength.ToString());
            EditorGUILayout.LabelField("Agility", unitController.agility.ToString());
            EditorGUILayout.LabelField("Intelligence", unitController.intelligence.ToString());
        }
        
        private void OnSceneGUI()
        {
            var unitController = Target;
            
            Handles.color = Color.green;
            var position = unitController.transform.position;
            Handles.DrawWireDisc(position, Vector3.up, unitController.interactRange);
            
            Handles.color = Color.red;
            Handles.DrawWireDisc(position, Vector3.up, unitController.vision);
            
            Handles.Label(position + Vector3.up * 2, unitController.name);
        }
    }
}