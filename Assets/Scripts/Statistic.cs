using System;
using System.Collections.Generic;
using UnityEditor;

namespace CM.Units
{
    [Serializable]
    public struct Statistic<T>
    {
        public T BaseValue;
        public Observable<T> Value;

        public Statistic(T value)
        {
            BaseValue = value;
            Value = new Observable<T>(value);
        }

        public static implicit operator T(Statistic<T> stat)
        {
            return stat.Value.Value;
        }

        public static implicit operator Statistic<T>(T value)
        {
            return new Statistic<T>(value);
        }
    }
}