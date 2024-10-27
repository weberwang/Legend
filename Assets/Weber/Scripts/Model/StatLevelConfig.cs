using System;
using GameCreator.Runtime.Stats;
using UnityEngine;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "StatLevelConfig", menuName = "Weber/Stat/StatLevelConfig", order = 0)]
    public class StatLevelConfig : ScriptableObject
    {
        public StatLevel[] statLevels;
    }

    [Serializable]
    public class StatLevel
    {
        public Stat stat;
        public RarityValue[] rarityValues;
    }

    [Serializable]
    public class RarityValue
    {
        public LuckConfig.SkillRarity rarity;
        public float value;
    }
}