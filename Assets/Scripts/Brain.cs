using System;
using UnityEngine;

namespace CM.Units
{
    [System.Serializable]
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
    
    
    /// <summary>
    /// Tried to accomplish some sort of inheritance with ScriptableObjects, but I'm struggling with initializing the
    /// different types of brains. I'm not sure if this is the right approach.
    /// </summary>
    public class BrainData : ScriptableObject
    {
        
    }
    
    [CreateAssetMenu(fileName = "BoarBrainData", menuName = "CM/Brains/BoarBrainData")]
    public class BoarBrainData : BrainData { }
    
    [CreateAssetMenu(fileName = "PlayerBrainData", menuName = "CM/Brains/PlayerBrainData")]
    public class PlayerBrainData : BrainData { }
    
    [CreateAssetMenu(fileName = "SoldierBrainData", menuName = "CM/Brains/SoldierBrainData")]
    public class SoldierBrainData : BrainData { }
}