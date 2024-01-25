using UnityEngine;

namespace CM.Units
{
    /// <summary>
    /// Tried to accomplish some sort of inheritance with ScriptableObjects, but I'm struggling with initializing the
    /// different types of brains. I'm not sure if this is the right approach.
    /// </summary>
    
    public abstract class BrainData : ScriptableObject
    {
        public abstract Brain Brain { get; }
        public abstract void Initialize(UnitStateController stateController, UnitController unitController);
    }
}