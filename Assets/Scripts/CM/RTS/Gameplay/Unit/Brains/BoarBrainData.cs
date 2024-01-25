using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(fileName = "BoarBrainData", menuName = "CM/Brains/BoarBrainData")]
    public class BoarBrainData : BrainData
    {
        [SerializeField]
        public BoarBrain brain = new();
        public override Brain Brain => brain;
        
        public override void Initialize(UnitStateController stateController, UnitController unitController)
        { 
            brain.Initialize(stateController, unitController);
        }
    }
}