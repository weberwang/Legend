using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Domain;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Unit
{
    public class Hero : CharacterUnit
    {
        public float PickRadius { get; private set; }

        protected override void OnCreate()
        {
            Character.IsPlayer = true;
            UpdateAttributes();
        }

        protected override void Death()
        {
            base.Death();
            //todo 检测是否还有复活
        }


        public override void UpdateStat(SkillEffectStatValue skillEffectStatValue)
        {
            base.UpdateStat(skillEffectStatValue);
            switch (skillEffectStatValue.stat.ID.ToString())
            {
                case Constants.TRAITS_PICK_DISTANCE:
                    PickRadius = GetRuntimeStatDataValue(Constants.TRAITS_PICK_DISTANCE);
                    break;
            }
        }

        private void UpdateAttributes()
        {
            var heroData = HeroManager.Instance.GetHeroData(ID);
            foreach (var skillEffectStatValue in heroData.skillValues)
            {
                GetRuntimeStatData(skillEffectStatValue.stat.ID).AddModifier(ModifierType.Constant, skillEffectStatValue.value);
            }

            //根据技能升级初始化属性
            foreach (var instanceHeroSkillValue in HeroManager.Instance.HeroSkillValues)
            {
                var levelValue = HeroManager.Instance.GetSkillLevelValue(instanceHeroSkillValue.Key);
                GetRuntimeStatData(instanceHeroSkillValue.Key).AddModifier(ModifierType.Constant, instanceHeroSkillValue.Value * levelValue.value);
            }

            PickRadius = GetRuntimeStatDataValue(Constants.TRAITS_PICK_DISTANCE);
        }

        private void UpgradeLevel()
        {
            var skillDatas = ChoiceSkillDatas();
        }

        public void AddExp(int xp)
        {
            var xpGain = GetRuntimeStatDataValue(Constants.TRAITS_XP_GAIN);
            var xpData = GetRuntimeStatData(Constants.TRAITS_XP);
            xpData.AddModifier(ModifierType.Constant, (1 + xpGain) * xp);
            //todo 检测是否升级,播放升级特效，弹出升级面板
        }
    }
}