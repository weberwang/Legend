using System;
using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Legend.SkillHitEffect;
using Weber.Scripts.Legend.Unit;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "AttackSkillData", menuName = "Weber/Skill/AttackSkillData", order = 1)]
    public class AttackSkillData : SkillData
    {
        public enum AttackTimes
        {
            Single,
            Multiple,
        }

        public CountDown countDown;
        public SkillType skillType = SkillType.Melee;

        [ShowIf("CheckIsWeapon")] public TWeapon weaponAsset;

        public BattleProp battlePropPrefab; //技能产生的节点，可以是武器，也可以是召唤物

        [SerializeField] private BaseSkillHitEffect[] skillHitEffects; //技能的攻击效果

        [NonSerialized] private BattleProp _battleProp;

        private List<BaseSkillHitEffect> _learnedSkillHitEffects = new List<BaseSkillHitEffect>();

        public List<BaseSkillHitEffect> LearnedSkillHitEffects => _learnedSkillHitEffects;

        private bool CheckIsWeapon()
        {
            return skillType == SkillType.Melee || skillType == SkillType.Shooter;
        }

        public void MergeHitEffect()
        {
            _learnedSkillHitEffects.AddRange(skillHitEffects);
        }


        public override void Learn(CharacterUnit characterUnit, BaseSkillHitEffect baseSkillHitEffect = null)
        {
            base.Learn(characterUnit, baseSkillHitEffect);
            CreateBattleProp(characterUnit);
            if (baseSkillHitEffect != null)
            {
                for (int i = 0; i < _learnedSkillHitEffects.Count; i++)
                {
                    if (_learnedSkillHitEffects[i].hitEffectType == baseSkillHitEffect.hitEffectType) return;
                }

                _learnedSkillHitEffects.Add(baseSkillHitEffect);
            }
        }

        private void CreateBattleProp(CharacterUnit characterUnit)
        {
            if (battlePropPrefab != null && _battleProp == null)
            {
                var propInstance = PoolManager.Instance.Pick(battlePropPrefab.gameObject, 1);
                var prop = propInstance.Get<BattleProp>();
                prop.SetUnitTarget(characterUnit, this);
                characterUnit.AddBattleProp(prop);
                _battleProp = prop;
            }
        }

        protected override void UpdateSkill(CharacterUnit characterUnit, SkillEffectStatValue learnSkill)
        {
            CreateBattleProp(characterUnit);
            if (_battleProp != null) _battleProp.UpdateSkill(learnSkill);
        }
    }


    public enum SkillType
    {
        None,
        Melee,
        Shooter,
        Spell,
    }
}