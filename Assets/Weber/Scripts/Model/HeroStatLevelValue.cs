using System;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Unit
{
    [CreateAssetMenu(fileName = "HeroStatLevelValue", menuName = "Weber/HeroStatLevelValue", order = 0)]
    public class HeroStatLevelValue : ScriptableObject
    {
        public int maxLevel = 4;
        public HeroBaseValue heroBaseValue;
    }


    [Serializable]
    public class HeroBaseValue
    {
        public SkillEffectStatValue[] skillValues;
    }
}