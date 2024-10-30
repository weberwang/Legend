using System;
using GameCreator.Runtime.Stats;

namespace Weber.Scripts.Model
{
    public class UpgradeSkillData
    {
        public SkillData skillData;
        public LuckConfig.SkillRarity skillRarity;
        public SkillEffectStatValue skillEffectStatValue;

        public UpgradeSkillData(SkillData skillData, LuckConfig.SkillRarity skillRarity, SkillEffectStatValue skillEffectStatValue)
        {
            this.skillData = skillData;
            this.skillRarity = skillRarity;
            this.skillEffectStatValue = skillEffectStatValue;
        }
    }
}