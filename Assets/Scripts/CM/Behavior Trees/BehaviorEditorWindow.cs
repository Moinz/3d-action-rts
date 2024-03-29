#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CM.Behavior_Trees
{
    public class BehaviorEditorWindow : GraphViewEditorWindow
    {
        private BehaviorGraphView graphView;

        [MenuItem("Window/CM/Behavior Editor")]
        public static void OpenBehaviorEditorWindow()
        {
            var window = GetWindow<BehaviorEditorWindow>();
            window.titleContent = new GUIContent("Behavior Editor");
        }

        GraphView CreateGraphView()
        {
            return new BehaviorGraphView(this);
        }

        public void CreateGUI()
        {
            var behaviorEditor = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/Behavior Editor.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/Behavior Editor.uss");
            
            var root = rootVisualElement;
            var behaviorVisualTree = behaviorEditor.Instantiate();
            
            behaviorVisualTree.styleSheets.Add(styleSheet);
            root.Add(behaviorVisualTree);
        }
    }
}
#endif