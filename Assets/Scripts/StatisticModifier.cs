using System;

namespace CM.Units
{
    [Serializable]
    public class StatisticModifier<T>
    {
        public T Value;

        public StatisticModifier(T value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}