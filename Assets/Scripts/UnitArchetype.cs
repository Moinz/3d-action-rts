using UnityEngine;

namespace CM.Units
{
    [CreateAssetMenu(fileName = "UnitArchetype", menuName = "CM/Units/Archetype")]
    public class UnitArchetype : ScriptableObject
    {
        public string name;
        public UnitStatistics statistics;
    }
}