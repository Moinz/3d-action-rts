using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(fileName = "PlayerBrainData", menuName = "CM/Brains/PlayerBrainData")]
    public class PlayerBrainData : BrainData
    {
        [SerializeField]
        public PlayerBrain brain = new();
        public override Brain Brain => brain;

        public override void Initialize(UnitStateController stateController, UnitController unitController)
        {
            brain.Initialize(stateController, unitController);
        }
    }
}