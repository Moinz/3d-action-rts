using System;
using TriInspector;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

using UnityEngine.UIElements;

namespace CM.Units
{
    public class StatisticAttribute : Attribute
    {
        public Type StatisticType { get; }

        public StatisticAttribute(Type statisticType)
        {
            if (!statisticType.IsGenericType || statisticType.GetGenericTypeDefinition() != typeof(Statistic<>))
                throw new ArgumentException("Invalid type for StatisticAttribute. It must be a Statistic<> type.", nameof(statisticType));

            StatisticType = statisticType;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StatisticAttribute))]
    public class StatisticAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            var baseValueField = new PropertyField(property.FindPropertyRelative("BaseValue"));
            var valueField = new PropertyField(property.FindPropertyRelative("Value"));
            
            container.Add(baseValueField);
            container.Add(valueField);
            
            return container;
        }
    }
#endif
    
    [Serializable]
    [DeclareHorizontalGroup("Statistics")]
    public struct Statistic<T>
    {
        [Group("Statistics")]
        public T BaseValue;
        
        [ReadOnly, Group("Statistics")]
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