using UnityEngine;

namespace CM.Units
{
    public class GlobalUnitSettings : ScriptableObject
    {
        private static GlobalUnitSettings _instance;

        public static GlobalUnitSettings Instance
        {
            get
            {
                if (!_instance)
                    _instance = CreateInstance<GlobalUnitSettings>();
                
                return _instance;
            }
        }

        public float interactRange = 3f;
    }
}