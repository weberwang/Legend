using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    public class MoveSpeedProp : BattleProp
    {
        public override void OnActive()
        {
            base.OnActive();
            Debug.Log("加速额外速度");
            var moveSpeedStat = CharacterUnit.GetRuntimeStatData(TraitsID.TRAITS_MOVE_SPEED);
            moveSpeedStat.AddModifier(ModifierType.Percent, SkillData.GetSkillEffectStatValueWithStatID(TraitsID.TRAITS_MOVE_SPEED).value);
        }

        public override void OnCooldown()
        {
            Debug.Log("移除额外速度");
            var moveSpeedStat = CharacterUnit.GetRuntimeStatData(TraitsID.TRAITS_MOVE_SPEED);
            moveSpeedStat.RemoveModifier(ModifierType.Percent, SkillData.GetSkillEffectStatValueWithStatID(TraitsID.TRAITS_MOVE_SPEED).value);
        }
    }
}