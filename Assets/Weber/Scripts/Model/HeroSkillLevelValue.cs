using System;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Unit
{
    [CreateAssetMenu(fileName = "HeroSkillLevelValue", menuName = "Weber/HeroSkillLevelValue", order = 0)]
    public class HeroSkillLevelValue : ScriptableObject
    {
        public int maxLevel = 4;
        public HeroBaseValue heroBaseValue;
    }


    [Serializable]
    public class HeroBaseValue
    {
        // public int armor = 20;
        // public float cooldown = -0.03f;
        // public float criticalChance = 0.03f;
        // public float criticalDamage = 0.07f;
        // public float damage = 0.05f;
        // public int health = 150;
        // public int healthRegen = 3;
        // public int luck = 4;
        // public float moveSpeed = 0.04f;
        // public float pickDistance = 0.1f;
        // public int rerolls = 1;
        // public float spellSize = 0.04f;
        // public float xpGain = 0.03f;
        
        public SkillEffectStatValue[] skillValues;
    }
}