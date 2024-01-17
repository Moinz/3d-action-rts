using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace CM.WorldGrid
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class CollapseButton : EditorToolbarButton
    {
        // This ID is used to populate toolbar elements.
        public const string id = "WorldGrid/CollapseButton";
    
        public CollapseButton()
        {
            text = "Collapse";
            tooltip = "Collapse the grid";
            icon = EditorGUIUtility.IconContent("Node3").image as Texture2D;
            clicked += OnClick;
        }

        private void OnClick()
        {
            var controller = LevelController.Instance;
            controller._waveFunctionCollapse.Collapse();
        }
    }
}