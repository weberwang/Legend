using System;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using UnityEngine.Playables;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Game;
using Weber.Scripts.Legend.Game.Items;
using Weber.Scripts.Model;
using Weber.Widgets.Popup;
using Math = Unity.Physics.Math;

namespace Weber.Scripts.Legend.Unit
{
    public class Hero : CharacterUnit
    {
        public float PickRadius { get; private set; }

        protected override void OnCreate()
        {
            Character.IsPlayer = true;
            _characterData = HeroManager.Instance.GetHeroData(ID);
            Character.Motion.Radius = _characterData.radius;
            UpdateAttributes();
            LearnSkill();
        }

        private async void LearnSkill()
        {
            await UniTask.Delay(1000);
            UpgradeLevel();
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
                case TraitsID.TRAITS_PICK_DISTANCE:
                    PickRadius = GetRuntimeStatValue(TraitsID.TRAITS_PICK_DISTANCE);
                    break;
                case TraitsID.TRAITS_MAX_HEALTH:
                    GetRunTimeAttributeData(TraitsID.TRAITS_HEALTH).Value += skillEffectStatValue.value;
                    break;
            }
        }

        private void UpdateAttributes()
        {
            foreach (var skillEffectStatValue in _characterData.skillValues)
            {
                var statID = skillEffectStatValue.stat.ID.ToString();
                GetRuntimeStatData(statID).ClearModifiers();
                GetRuntimeStatData(statID).AddModifier(ModifierType.Constant, skillEffectStatValue.value);
            }

            //根据技能升级初始化属性
            foreach (var instanceHeroSkillValue in HeroManager.Instance.HeroSkillValues)
            {
                var levelValue = HeroManager.Instance.GetSkillLevelValue(instanceHeroSkillValue.Key);
                GetRuntimeStatData(instanceHeroSkillValue.Key).AddModifier(ModifierType.Constant, instanceHeroSkillValue.Value * levelValue.value);
            }

            PickRadius = GetRuntimeStatValue(TraitsID.TRAITS_PICK_DISTANCE);
            GetRunTimeAttributeData(TraitsID.TRAITS_HEALTH).Value = GetRuntimeStatData(TraitsID.TRAITS_MAX_HEALTH).Value;
        }

        private void UpgradeLevel()
        {
            var skillDatas = ChoiceSkillDatas();
            PopupManager.ShowPopup(PopupName.POPUP_CHOICE_SKILL, skillDatas);
        }

        public void AddExp(int xp)
        {
            var levelStat = GetRuntimeStatData(TraitsID.TRAITS_LEVEL);
            var currentLevel = levelStat.Value;
            var xpGain = GetRuntimeStatValue(TraitsID.TRAITS_XP_GAIN);
            var xpData = GetRunTimeAttributeData(TraitsID.TRAITS_XP);
            xpData.Value += ((1 + xpGain) * xp);
            //todo 检测是否升级,播放升级特效，弹出升级面板
            if (levelStat.Value > currentLevel)
            {
                UpgradeLevel();
            }

            Signals.Emit(new SignalArgs(SignalNames.PICK_XP, gameObject));
        }

        public UpgradeSkillData[] RefreshChoiceSkill()
        {
            var rerolls = GetRuntimeStatValue(TraitsID.TRAITS_REROLLS);
            if (rerolls > 0)
            {
                return ChoiceSkillDatas();
            }

            return null;
        }
    }
}