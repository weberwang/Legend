using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Shooter;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Legend.SkillHitEffect;
using Weber.Scripts.Model;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Legend.Unit
{
    [RequireComponent(typeof(Character))]
    public class CharacterUnit : MonoBehaviour
    {
        [SerializeField] private LayerMask deathLayer;
        [SerializeField] private SkillManager m_SkillManager;

        [SerializeField] private AnimationClip m_HitClip;
        [SerializeField] private AnimationClip m_DeathClip;
        public Character Character { get; private set; }

        protected Traits traits;
        public RuntimeAttributeData HealthAttributes { get; private set; }
        public RuntimeAttributeData DamageAttributes { get; private set; }

        private List<BattleProp> _battleProps = new List<BattleProp>();

        private List<BaseSkillHitEffect> _skillHitEffects = new List<BaseSkillHitEffect>();

        public float Health => (float)HealthAttributes.Value;

        private void Awake()
        {
            Character = GetComponent<Character>();
            traits = GetComponent<Traits>();
            HealthAttributes = traits.RuntimeAttributes.Get(Constants.TRAITS_HEALTH);
            HealthAttributes.EventChange += OnHealthChange;
            Character.EventDie += OnDie;
            OnCreate();
        }


        protected virtual void OnCreate()
        {
        }

        private void Update()
        {
            OnUpdate();
            UpdateHitEffect();
        }

        protected virtual void OnUpdate()
        {
        }

        private void UpdateHitEffect()
        {
            for (int i = _skillHitEffects.Count - 1; i > 0; i--)
            {
                var skillHitEffect = _skillHitEffects[i];
                if (skillHitEffect.IsEnd)
                {
                    _skillHitEffects.RemoveAt(i);
                }
                else
                {
                    skillHitEffect.OnUpdate();
                }
            }
        }


        public void OnShootHit()
        {
            var shotData = ShooterWeapon.LastShotData;
            var source = shotData.Source;
            var prop = shotData.Prop;
            var sourceUnit = source.Get<CharacterUnit>();
            var battleProp = prop.Get<BattleProp>();
            if (battleProp)
            {
                OnSkillHit(battleProp);
            }
        }

        public void OnMeleeHit()
        {
        }

        public void OnHurt(float damage, bool ignoreArmor = false)
        {
            Debug.Log("OnHurt:" + damage);
            var armor = ignoreArmor ? 0 : GetStat(Constants.TRAITS_ARMOR);
            var realDamage = damage - armor;
            HealthAttributes.Value -= realDamage;
        }

        public void OnSkillHit(BattleProp battleProp)
        {
            var damage = battleProp.GetStat(Constants.TRAITS_DAMAGE) * Random.Range(0.8f, 1.2f);
            OnHurt(damage);
            if (HealthAttributes.Value > 0)
            {
                EffectWithSkill(battleProp);
            }
        }

        public void ChangeDamage(float damage)
        {
            DamageAttributes.Value = damage;
        }

        private void OnHealthChange(IdString arg1, double health)
        {
            if (health <= 0)
            {
                Death();
            }
            else
            {
                Hit();
            }
        }

        private void OnDie()
        {
            for (int i = 0; i < _battleProps.Count; i++)
            {
                Destroy(_battleProps[i].gameObject);
            }

            _battleProps.Clear();
        }

        private async void Death()
        {
            Character.IsDead = true;
            await PlayGesture(m_DeathClip);
            gameObject.SetActive(false);
        }

        private async void Hit()
        {
            await PlayGesture(m_HitClip);
        }

        private void EffectWithSkill(BattleProp battleProp)
        {
            var skillData = battleProp.SkillData;
            var skillHitEffects = skillData.LearnedSkillHitEffects;
            for (int i = 0; i < skillHitEffects.Count; i++)
            {
                skillHitEffects[i].SetTarget(this, skillData);
            }
        }


        private async UniTask PlayGesture(AnimationClip animationClip)
        {
            if (animationClip == null) return;
            ConfigGesture configuration = new ConfigGesture(
                0, animationClip.length - 0.05f,
                1, false,
                0.1f,
                0.1f
            );

            Task gestureTask = Character.Gestures.CrossFade(
                animationClip, null, BlendMode.Blend,
                configuration, false
            );
            await gestureTask;
        }

        public void UpdateAttribute(string attributeID, float value)
        {
            traits.RuntimeAttributes.Get(attributeID).Value += value;
        }

        public double GetAttribute(string attributeID)
        {
            var attribute = traits.RuntimeAttributes.Get(attributeID);
            if (attribute != null)
            {
                return attribute.Value;
            }

            return 0;
        }

        public double GetStat(string statID)
        {
            var stat = traits.RuntimeStats.Get(statID);
            if (stat != null)
            {
                return stat.Value;
            }

            return 0;
        }

        public void AddBattleProp(BattleProp battleProp)
        {
            _battleProps.Add(battleProp);
        }
    }
}