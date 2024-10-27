using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Domain;

namespace Weber.Scripts.Legend.Unit
{
    public class Hero : CharacterUnit
    {
        [SerializeField] private UniqueID _heroID = new UniqueID(UniqueID.GenerateID());

        protected override void OnCreate()
        {
            Character.IsPlayer = true;
            UpdateAttributes();
        }

        protected override void OnDie()
        {
            base.OnDie();
            //todo 检查是否还有复活次数
        }

        private void UpdateAttributes()
        {
            var heroData = HeroManager.Instance.GetHeroData(_heroID);
            foreach (var skillEffectStatValue in heroData.skillValues)
            {
                GetRuntimeStatData(skillEffectStatValue.stat.ID.String).AddModifier(skillEffectStatValue.changeValueType, skillEffectStatValue.value);
            }

            //根据技能升级初始化属性
            foreach (var instanceHeroSkillValue in HeroManager.Instance.HeroSkillValues)
            {
                var levelValue = HeroManager.Instance.GetSkillLevelValue(instanceHeroSkillValue.Key);
                GetRuntimeStatData(instanceHeroSkillValue.Key).AddModifier(levelValue.changeValueType, instanceHeroSkillValue.Value * levelValue.value);
            }
        }

        private void UpgradeLevel()
        {
            var skillDatas = ChoiceSkillDatas();
            
        }
    }
}