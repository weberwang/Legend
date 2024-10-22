using System;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.SkillHitEffect
{
    public class BaseSkillHitEffect : ScriptableObject
    {
        [SerializeField] private HitEffectType _hitEffectType;
        [SerializeField] private SkillEffectStatValue[] skillValues;
        [SerializeField] private GameObject _effectPrefab;
        [SerializeField] private CountDown _countDown;
        protected CharacterUnit _characterUnit; //受影响的角色
        protected SkillData _skillData; //技能来源
        public HitEffectType hitEffectType => _hitEffectType;

        public bool IsEnd { get; private set; }
        protected GameObject effectInstance;


        public void SetTarget(CharacterUnit characterUnit, SkillData skillData)
        {
            _characterUnit = characterUnit;
            _skillData = skillData;
            LoadEffect();
            _countDown.Start();
        }

        private void LoadEffect()
        {
            effectInstance = PoolManager.Instance.Pick(_effectPrefab, 1);
            effectInstance.transform.parent = _characterUnit.transform;
        }

        public virtual void OnUpdate()
        {
            if (IsEnd) return;
            if (_countDown.OnUpdate())
            {
                ActiveHitEffect();
            }

            if (_countDown.Ended)
            {
                DeactiveHitEffect();
            }
        }

        protected virtual void ActiveHitEffect()
        {
            IsEnd = false;
        }

        protected virtual void DeactiveHitEffect()
        {
            IsEnd = true;
        }
    }

    [Serializable]
    public enum HitEffectType
    {
        None,

        //冰冻
        Freeze,

        //燃烧
        Burn,

        //中毒
        Poison,

        //眩晕
        Dizzy,

        //击退
        KnockBack,

        //减速
        Slow,
    }
}