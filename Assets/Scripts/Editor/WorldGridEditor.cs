using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CM.WorldGrid
{
    [CustomEditor(typeof(WorldGrid))]
    public class WorldGridEditor : Editor
    {
        private WorldGrid Target => target as WorldGrid;
        public static WorldSelection _selection;
        private static Vector3Int _selectionCenter;

        private void OnEnable()
        {
            _selection = new WorldSelection(_selectionCenter, 3);

            Target.Init();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.LabelField("Selection");
            EditorGUILayout.Vector3Field("Center", _selection.center);
            EditorGUILayout.IntField("Size", _selection.size);
        }

        public void OnSceneGUI()
        {
            Debug_Controls();
            DrawSelectedCells();
        }

        private void Debug_Controls()
        {
            if (!Keyboard.current.leftCtrlKey.isPressed || !Mouse.current.leftButton.isPressed) 
                return;
            
            SelectCell();
            _selection = new WorldSelection(_selectionCenter, 3);
        }

        private void SelectCell()
        {
            var camera = SceneView.GetAllSceneCameras()[0];

            if (!camera)
                return;
            
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Default"));
            Debug.DrawRay(ray.origin, ray.direction, Color.magenta);
            var point = hit.point;
            _selectionCenter = new Vector3Int(Mathf.RoundToInt(point.x), 0, Mathf.RoundToInt(point.z));
        }

        private void DrawSelectedCells()
        {
            Handles.color = Color.red;
            foreach (var cell in _selection.Cells)
            {
                DrawCell(cell.position);
            }
            
            Handles.color = Color.gray;
            DrawCell(_selectionCenter);
        }

        private void DrawCell(Vector3Int pos)
        {
            var worldPos = Target.transform.position + new Vector3(pos.x, 0f, pos.z);
            
            Handles.DrawWireCube(worldPos, Vector3.one);
            Handles.Label(worldPos, pos.ToString());
        }
    }
}