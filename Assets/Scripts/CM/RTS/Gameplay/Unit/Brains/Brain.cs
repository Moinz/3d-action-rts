using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CM.Units
{
    [Serializable]
    public abstract class Brain
    {
        public abstract int TickRate { get; }
        
        public UnitStateController _stateController;
        public UnitController _unitController;
        
        public Inventory inventory => _unitController._inventory;
        
        public GameObject Target
        {
            get => _stateController.target;
            set => _stateController.target = value;
        }

        public LayerMask searchMask;

        public abstract void Tick();
        
        public abstract void Initialize(UnitStateController stateController, UnitController unitController);
    }

    public static class BrainExtensions
    {
        
        internal static Vector3 RandomPointInCircle(Vector3 center, float radius)
        {
            var point = Random.insideUnitCircle * radius;
        
            return center + new Vector3(point.x, 0f, point.y);
        }

    }
}