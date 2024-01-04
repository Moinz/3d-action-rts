using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.InputSystem;

using HandleUtility = UnityEditor.HandleUtility;

namespace CM.WorldGrid
{
    [EditorToolbarElement(id, typeof(SceneView))]
    public class CollapseAllButton : EditorToolbarButton
    {
        // This ID is used to populate toolbar elements.
        public const string id = "WorldGrid/CollapseAllButton";
    
        public CollapseAllButton()
        {
            text = "Collapse";
            tooltip = "Collapse the grid";
            icon = EditorGUIUtility.IconContent("Node2").image as Texture2D;
            clicked += OnClick;
        }

        private void OnClick()
        {
            var controller = LevelController.Instance;
            controller._waveFunctionCollapse.Collapse();
        }
    }
    
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
    
    // All Overlays must be tagged with the OverlayAttribute
// IconAttribute provides a way to define an icon for when an Overlay is in collapsed form. If not provided, the first
// two letters of the Overlay name will be used.
// Toolbar overlays must inherit `ToolbarOverlay` and implement a parameter-less constructor. The contents of a toolbar
// are populated with string IDs, which are passed to the base constructor. IDs are defined by
// EditorToolbarElementAttribute.
    // ToolbarOverlay implements a parameterless constructor, passing 2 EditorToolbarElementAttribute IDs. This will
        // create a toolbar overlay with buttons for the CreateCubes and DropdownToggleExample examples.
        // This is the only code required to implement a toolbar overlay. Unlike panel overlays, the contents are defined
        // as standalone pieces that will be collected to form a strip of elements.
    
    [Overlay(typeof(SceneView), "World Grid")]
    [Icon("BA")]
    public class WorldGridToolOverlay : ToolbarOverlay
    {
        WorldGridToolOverlay() : base(
            CollapseButton.id,
            CollapseAllButton.id)
        {}
    }
    
    
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

    [System.Serializable]
    public struct WorldSelection
    {
        internal Vector3Int center;
        internal int size;
        private WorldCell[] cells;
        
        public WorldCell[] Cells => cells ?? GetSelectionCells();

        public WorldSelection(Vector3Int center, int size = 3)
        {
            this.center = center;
            this.size = size;
            cells = null;
        }
        
        public WorldSelection(int x, int y, int z, int size)
        {
            center = new Vector3Int(x, y, z);
            this.size = size;
            cells = null;
        }

        public WorldCell[] GetCellsAroundCenter()
        {
            cells = new WorldCell[8];

            var grid = WorldGrid.Instance;
            
            // Get Cells Counterclockwise
            cells[0] = grid.GetCell(center.x + 1, center.y, center.z);
            cells[1] = grid.GetCell(center.x + 1, center.y, center.z + 1);
            cells[2] = grid.GetCell(center.x, center.y, center.z + 1);
            cells[3] = grid.GetCell(center.x - 1, center.y, center.z + 1);
            cells[4] = grid.GetCell(center.x - 1, center.y, center.z);
            cells[5] = grid.GetCell(center.x - 1, center.y, center.z - 1);
            cells[6] = grid.GetCell(center.x, center.y, center.z - 1);
            cells[7] = grid.GetCell(center.x + 1, center.y, center.z - 1);
            
            return cells;
        }
        
        private WorldCell[] GetSelectionCells()
        {
            var c = new List<WorldCell>(size * size);
            
            c.Add(new WorldCell(center));
            c.AddRange(GetCellsAroundCenter());
            
            cells = c.ToArray();
            return cells;
        }
    }

    public class WorldGrid : MonoBehaviour
    {
        public int size;
        public WorldCell[] cells;

        private static WorldGrid _instance;

        public static WorldGrid Instance
        {
            get
            {
                _instance ??= FindObjectOfType<WorldGrid>();
                return _instance;
            } 
        }

        private void Awake()
        {
            Init();

            _instance = this;
        }

        internal void Init()
        {
            cells = new WorldCell[size * size];
            var halfSize = Mathf.RoundToInt(size * 0.5f);

            for (int i = 0; i < cells.Length; i++)
            {
                var position = new Vector3Int(i % size - halfSize, 0, i / size - halfSize);
                cells[i] = new WorldCell(position);
            }
        }
        
        public WorldCell GetCell(int x, int y, int z)
        {
            var position = new Vector3Int(x, y, z);

            return cells.FirstOrDefault(x => x.position == position);
        }
    }

    // TODO: Implement a chunk, the middle step between the Grid and the Cell
    // TODO: Will optimize the grid by limiting the amount of data we need to process. 
    
    [Serializable]
    public struct WorldCell
    {
        public Vector3Int position;
    
        public WorldCell(Vector3Int position)
        {
            this.position = position;
        }
    }
}

