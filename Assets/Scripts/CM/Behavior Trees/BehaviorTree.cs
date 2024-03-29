using UnityEngine;

namespace CM.Behavior_Trees
{

    public class BehaviorTree : MonoBehaviour
    {
        private BehaviorNode rootNode;

        public void Initialize(BehaviorNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public void Tick()
        {
            rootNode.Tick();
        }
    }
}