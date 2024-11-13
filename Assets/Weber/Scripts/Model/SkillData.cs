using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Legend.SkillHitEffect;
using Weber.Scripts.Legend.Unit;
using Attribute = GameCreator.Runtime.Stats.Attribute;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "Weber/Skill/SkillData", order = 0)]
    public class SkillData : ScriptableObject
    {
        public UniqueID ID = new UniqueID(UniqueID.GenerateID());
        public ClassType classType = ClassType.Special;
        public int maxLevel;
        public string skillName;
        [TextArea] public string description;
        public Sprite icon;
        public SkillEffectStatValue[] stats;

        // [field: NonSerialized] public bool Learned { get; private set; }
        [field: NonSerialized] public int Level { get; private set; }

        // public virtual void Learn(CharacterUnit characterUnit, BaseSkillHitEffect baseSkillHitEffect = null)
        // {
        //     if (Learned) return;
        //     Learned = true;
        //     Level = 1;
        // }

        public virtual bool Upgrade(CharacterUnit characterUnit, SkillEffectStatValue learnSkill = null, BaseSkillHitEffect skillHitEffect = null)
        {
            // if (!Learned) return false;
            if (maxLevel <= 0 || Level < maxLevel)
            {
                Level++;
                UpdateSkill(characterUnit, learnSkill, skillHitEffect);
                return true;
            }

            return false;
        }

        protected virtual void UpdateSkill(CharacterUnit characterUnit, SkillEffectStatValue learnSkill = null, BaseSkillHitEffect skillHitEffect = null)
        {
            characterUnit.UpdateStat(learnSkill);
        }

        public SkillEffectStatValue ChoiceSkillEffectStatValue()
        {
            var index = Random.Range(0, stats.Length);
            return stats[index];
        }

        public SkillEffectStatValue GetSkillEffectStatValueWithStatID(string statID)
        {
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].stat.ID.String == statID)
                {
                    return stats[i];
                }
            }

            return null;
        }
    }

    //技能影响的属性
    [Serializable]
    public class SkillEffectStatValue
    {
        public Stat stat;
        public float value;
        public bool ignoreInitialValue = false;
        public ModifierType changeValueType = ModifierType.Constant;

        public SkillEffectStatValue(Stat stat, float value, ModifierType changeValueType)
        {
            this.stat = stat;
            this.value = value;
            this.changeValueType = changeValueType;
        }

        public float Value(float changeValue)
        {
            switch (changeValueType)
            {
                case ModifierType.Constant:
                    return changeValue;
                case ModifierType.Percent:
                    return value * changeValue;
            }

            return 0;
        }

        public SkillEffectStatValue Clone()
        {
            return new SkillEffectStatValue(stat, value, changeValueType);
        }
    }

    [Serializable]
    public enum BaseValueType
    {
        //固定值
        Fixed, //百分比
        Percent
    }

    [Serializable]
    public enum ClassType
    {
        Special,
        Common,
        Base
    }
}