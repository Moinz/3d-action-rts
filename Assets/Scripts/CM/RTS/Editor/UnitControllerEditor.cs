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
            EditorGUILayout.BeginVertical();
            GUI.backgroundColor = Color.grey;
            DrawStatistic("Interact Range", unitController.interactRange, unitController.interactRange);
            DrawStatistic("Vision", unitController.vision, unitController._statistics.vision.BaseValue);
            
            EditorGUILayout.LabelField("Combat Statistics", EditorStyles.boldLabel);
            DrawStatistic("Attack", unitController.attack, unitController._statistics.attack.BaseValue);
            DrawStatistic("Attack Speed", unitController.attackSpeed, unitController._statistics.attackSpeed.BaseValue);
            DrawStatistic("Defense", unitController.defense, unitController._statistics.defense.BaseValue);
            
            EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
            DrawStatistic("Strength", unitController.strength, unitController._statistics.strength.BaseValue);
            DrawStatistic("Agility", unitController.agility, unitController._statistics.agility.BaseValue);
            DrawStatistic("Intelligence", unitController.intelligence, unitController._statistics.intelligence.BaseValue);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }

        private void DrawStatistic(string label, object value, object baseValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, value.ToString());
            GUI.enabled = false;
            EditorGUILayout.LabelField(baseValue.ToString());
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
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