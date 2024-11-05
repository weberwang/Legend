using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "LuckConfig", menuName = "Weber/Skill/LuckConfig")]
    public class LuckConfig : ScriptableObject
    {
        [Serializable]
        public class LuckThreshold
        {
            public int minLuck;  // 幸运值的最小值
            public int maxLuck;  // 幸运值的最大值
            public List<RarityChance> rarityChances; // 品质及对应的概率
        }

        [Serializable]
        public class RarityChance
        {
            public SkillRarity rarity;  // 技能的品质
            [Range(0f, 1f)] public float chance;  // 获得此品质的概率
        }

        public LuckThreshold[] luckThresholds; // 幸运区间配置表
        
        public enum SkillRarity
        {
            Common,     // 普通
            Rare,       // 罕见
            Epic,       // 史诗
            Legendary,  // 传说
            Mythical    // 神话
        }
    }
}