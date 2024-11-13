using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Model;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Unit
{
    public class Hero : CharacterUnit
    {
        public float PickRadius { get; private set; }

        public HeroData HeroData => _characterData as HeroData;

        private List<UpgradeSkillData[]> _upgradeSkillDatas = new List<UpgradeSkillData[]>();

        protected override void OnCreate()
        {
            Character.IsPlayer = true;
            _characterData = HeroManager.Instance.GetHeroData(ID);
            Character.Motion.Radius = _characterData.radius;
            UpdateAttributes();
        }

        protected override void Death()
        {
            base.Death();
            //todo 检测是否还有复活
            var reviveCount = GetRuntimeStatValue(TraitsID.TRAITS_REVIVE);
            if (reviveCount > 0)
            {
                //todo 复活
            }
            else
            {
                //todo 游戏结束
            }
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
                GetRuntimeStatData(statID).Base = skillEffectStatValue.value;
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
            _upgradeSkillDatas.Add(skillDatas);
            if (!PopupManager.Exist(PopupName.POPUP_CHOICE_SKILL))
            {
                ShowUpgradeSkillPopup();
            }
        }

        private async UniTask ShowUpgradeSkillPopup()
        {
            if (_upgradeSkillDatas.Count <= 0) return;
            var skillDatas = _upgradeSkillDatas[0];
            await PopupManager.ShowPopup(PopupName.POPUP_CHOICE_SKILL, skillDatas, () => { ShowUpgradeSkillPopup(); });
            _upgradeSkillDatas.RemoveAt(0);
        }

        public void AddExp(int xp)
        {
            var levelStat = GetRuntimeStatData(TraitsID.TRAITS_LEVEL);
            var currentLevel = levelStat.Value;
            var xpGain = GetRuntimeStatValue(TraitsID.TRAITS_XP_GAIN);
            var xpData = GetRuntimeStatData(TraitsID.TRAITS_XP);
            xpData.AddModifier(ModifierType.Constant, xp * (1 + xpGain));
            //todo 检测是否升级,播放升级特效，弹出升级面板
            Debug.LogFormat("增加经验:{0} 总经验:{1}  之前等级{2} 现在等级{3}", xp, xpData.Value, currentLevel, levelStat.Value);
            if (levelStat.Value > currentLevel)
            {
                UpgradeLevel();
                Signals.Emit(new SignalArgs(SignalNames.LEVEL_UP, gameObject));
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