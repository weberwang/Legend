using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "LearnSkillData", menuName = "Weber/Skill/LearnSkillData", order = 0)]
    public class LearnSkillData : ScriptableObject
    {
        public int[] specialSkillLevel;
        public int[] commonSkillLevel;
        public int[] skillHitEffectLevel;//达到等级可以激活的技能效果
    }

    [Serializable]
    public class HeroSkill
    {
        public UniqueID heroID = new UniqueID();
        public int[] specialSkillLevel;
    }
}