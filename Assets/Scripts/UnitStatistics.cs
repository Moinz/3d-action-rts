using System;

namespace CM.Units
{
    [Serializable]
    public class UnitStatistics
    {
        public Statistic<int> health;
        public Statistic<int> attack;
        public Statistic<int> defense;
        
        public Statistic<float> attackSpeed;
        public Statistic<int> vision;

        public Statistic<int> strength;
        public Statistic<int> agility;
        public Statistic<int> intelligence;
        
        public UnitStatistics()
        {
            health = 100;
            attack = 10;
            defense = 1;
            attackSpeed = 2f;
            vision = 4;
            strength = 10;
            agility = 10;
            intelligence = 10;
        }
        
        public UnitStatistics(UnitStatistics stats)
        {
            health = stats.health;
            attack = stats.attack;
            defense = stats.defense;
            attackSpeed = stats.attackSpeed;
            vision = stats.vision;
            strength = stats.strength;
            agility = stats.agility;
            intelligence = stats.intelligence;
        }
    }
}