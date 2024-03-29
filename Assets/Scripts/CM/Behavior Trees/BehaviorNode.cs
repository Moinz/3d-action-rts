namespace CM.Behavior_Trees
{
    public abstract class BehaviorNode
    {
        public enum NodeStatus
        {
            Running,
            Success,
            Failure
        }

        protected NodeStatus status;

        public BehaviorNode()
        {
            status = NodeStatus.Running;
        }

        public abstract NodeStatus Tick();
    }
}