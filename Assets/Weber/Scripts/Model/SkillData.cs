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
        [SerializeField] private UniqueID ID = new UniqueID(UniqueID.GenerateID());
        public int maxLevel;
        public string skillName;
        [TextArea] public string description;
        public Sprite icon;
        public SkillEffectStatValue[] stats;

        [field: NonSerialized] public bool Learned { get; private set; }
        [field: NonSerialized] public int Level { get; private set; }

        public virtual void Learn(CharacterUnit characterUnit, BaseSkillHitEffect baseSkillHitEffect = null)
        {
            if (Learned) return;
            Learned = true;
            Level = 1;
        }

        public virtual bool Upgrade(CharacterUnit characterUnit, LearnSkill learnSkill)
        {
            if (!Learned) return false;
            if (Level < maxLevel)
            {
                Level++;
                UpdateSkill(characterUnit, learnSkill);
                return true;
            }

            return false;
        }

        public virtual void UpdateSkill(CharacterUnit characterUnit, LearnSkill learnSkill)
        {
            characterUnit.UpdateAttribute(learnSkill.id, GetChangeValue(learnSkill));
        }

        public float GetChangeValue(LearnSkill learnSkill)
        {
            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].stat.ID.String == learnSkill.id)
                {
                    return stats[i].CalculateValue(learnSkill.value);
                }
            }

            return 0;
        }

        public SkillEffectStatValue ChoiceAttribute()
        {
            var index = Random.Range(0, stats.Length);
            return stats[index];
        }
    }

    //技能影响的属性
    [Serializable]
    public class SkillEffectStatValue
    {
        public Stat stat;
        public float value;
        public BaseValueType changeValueType = BaseValueType.Fixed;

        public float CalculateValue(float value)
        {
            switch (changeValueType)
            {
                case BaseValueType.Fixed:
                    return value;
                case BaseValueType.Percent:
                    return value * this.value;
                default:
                    return 0;
            }
        }
    }

    [Serializable]
    public struct SkillEffectAttribute
    {
        public Attribute attribute;
        public float value;
    }

    [Serializable]
    public struct LearnSkill
    {
        public string id;
        public float value;
    }

    [Serializable]
    public enum BaseValueType
    {
        //固定值
        Fixed, //百分比
        Percent
    }

    [Serializable]
    public enum EffectType
    {
        Attribute,
        Stat
    }
}