using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CM.WorldGrid
{

    [System.Serializable]
    public struct WorldSelection
    {
        public Vector3Int center;
        public int size;
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

    [ExecuteInEditMode]
    public class WorldGrid : MonoBehaviour
    {
        public int size;
        public WorldCell[] cells;

        private static WorldGrid _instance;

        public static WorldGrid Instance
        {
            get
            {
                _instance = _instance 
                                ? _instance 
                                : new GameObject("World Grid").AddComponent<WorldGrid>();
                
                return _instance;
            } 
        }

        private void Awake()
        {
            Init();

            _instance = this;
        }

        public void Init()
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

