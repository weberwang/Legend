using System;
using GameCreator.Runtime.Stats;

namespace Weber.Scripts.Model
{
    public class UpgradeSkillData
    {
        public string skillID;
        public LuckConfig.SkillRarity skillRarity;
        public SkillEffectStatValue skillEffectStatValue;

        public UpgradeSkillData(string skillID, LuckConfig.SkillRarity skillRarity, SkillEffectStatValue skillEffectStatValue)
        {
            this.skillID = skillID;
            this.skillRarity = this.skillRarity;
            this.skillEffectStatValue = skillEffectStatValue;
        }
    }
}