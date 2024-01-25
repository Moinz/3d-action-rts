using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(fileName = "SoldierBrainData", menuName = "CM/Brains/SoldierBrainData")]
    public class SoldierBrainData : BrainData
    {
        public SoldierBrain brain = new();
        public override Brain Brain => brain;

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            brain.Initialize(stateController, unitController);
        }
    }
}