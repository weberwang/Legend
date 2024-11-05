using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Shooter;
using GameCreator.Runtime.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Legend.SkillHitEffect;
using Weber.Scripts.Model;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Legend.Unit
{
    [RequireComponent(typeof(Character))]
    public class CharacterUnit : MonoBehaviour, ISignalReceiver
    {
        [SerializeField] private LayerMask _deathLayer;
        [SerializeField] private SkillManager _skillManager;

        public Character Character { get; private set; }

        protected Transform Transform;

        public int ID { get; private set; }

        protected Traits traits;
        public RuntimeAttributeData HealthAttributes { get; private set; }

        private List<BattleProp> _battleProps = new List<BattleProp>();

        private List<BaseSkillHitEffect> _skillHitEffects = new List<BaseSkillHitEffect>();

        protected CharacterData _characterData;
        public CharacterData CharacterData => _characterData;

        public float Health => (float)HealthAttributes.Value;
        public UnityAction<CharacterUnit> OnDeath;

        private void Awake()
        {
            Transform = transform;
            Character = GetComponent<Character>();
            traits = GetComponent<Traits>();
            HealthAttributes = GetRunTimeAttributeData(TraitsID.TRAITS_HEALTH);
            OnCreate();
            Signals.Subscribe(this, SignalNames.SPELL_HIT);
        }

        protected virtual void OnCreate()
        {
        }

        private void Start()
        {
            _skillManager.InitialSkill();
        }

        private void OnDestroy()
        {
            Signals.Unsubscribe(this, SignalNames.SPELL_HIT);
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


        public void OnShootHit(CharacterUnit target)
        {
            OnInitSkillHit(target);
        }

        public void OnMeleeHit(CharacterUnit target)
        {
            OnInitSkillHit(target);
        }

        private void OnInitSkillHit(CharacterUnit target)
        {
            var skillData = target._skillManager.InitSkillData;
            if (skillData is null)
            {
                Debug.Log("没有默认技能");
                return;
            }

            var battleProp = target.GetBattleProp(skillData.ID);
            if (battleProp)
            {
                OnSkillHit(battleProp);
            }
        }

        private void OnHurt(float damage, bool ignoreArmor = false)
        {
            var armor = ignoreArmor ? 0 : GetRuntimeStatValue(TraitsID.TRAITS_ARMOR);
            var realDamage = Mathf.FloorToInt(damage - armor);
            HealthAttributes.Value -= realDamage;
            if (HealthAttributes.Value <= 0)
            {
                Death();
            }
            else
            {
                Hit();
            }
        }

        public void OnSkillHit(BattleProp battleProp)
        {
            if (Character.IsDead) return;
            var damage = battleProp.GetRuntimeStatDataValue(TraitsID.TRAITS_DAMAGE) * Random.Range(0.9f, 1.1f);
            var critical = battleProp.GetRuntimeStatDataValue(TraitsID.TRAITS_CRITICAL);
            var criticalDamage = battleProp.GetRuntimeStatDataValue(TraitsID.TRAITS_CRITICAL_DAMAGE);
            if (Random.value < critical)
            {
                damage *= (1 + criticalDamage);
            }

            OnHurt(damage);
            if (HealthAttributes.Value > 0)
            {
                EffectWithSkill(battleProp);
            }
        }

        protected virtual async void Death()
        {
            if (Character.IsDead) return;
            Character.IsDead = true;
            OnDeath?.Invoke(this);
            for (int i = 0; i < _battleProps.Count; i++)
            {
                _battleProps[i].gameObject.SetActive(false);
            }

            _battleProps.Clear();
            await PlayGesture(_characterData.death);
            gameObject.SetActive(false);
        }

        private async void Hit()
        {
            //todo 显示伤害效果
            await PlayGesture(_characterData.hurt);
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

        public virtual void UpdateStat(SkillEffectStatValue skillEffectStatValue)
        {
            for (var i = 0; i < _battleProps.Count; i++)
            {
                _battleProps[i].UpdateSkill(skillEffectStatValue);
            }

            if (skillEffectStatValue.stat.ID.String == TraitsID.TRAITS_PICK_DISTANCE)
            {
                GetRuntimeStatData(skillEffectStatValue.stat.ID.ToString()).AddModifier(ModifierType.Percent, skillEffectStatValue.value);
            }
            else
            {
                GetRuntimeStatData(skillEffectStatValue.stat.ID.ToString()).AddModifier(ModifierType.Constant, skillEffectStatValue.value);
            }
        }

        public virtual void UpdateStat(string statID, float value, ModifierType modifierType)
        {
            GetRuntimeStatData(statID).AddModifier(modifierType, value);
        }

        public double GetRunTimeAttributeValue(string attributeID)
        {
            var attribute = traits.RuntimeAttributes.Get(attributeID);
            if (attribute is not null)
            {
                return attribute.Value;
            }

            return 0;
        }

        public RuntimeAttributeData GetRunTimeAttributeData(string attributeID)
        {
            return traits.RuntimeAttributes.Get(attributeID);
        }

        public float GetRuntimeStatValue(string statID)
        {
            var stat = traits.RuntimeStats.Get(statID);
            if (stat is not null)
            {
                return Convert.ToSingle(stat.Value);
            }

            return 0;
        }

        public RuntimeStatData GetRuntimeStatData(string statID)
        {
            return traits.RuntimeStats.Get(statID);
        }

        public void AddBattleProp(BattleProp battleProp)
        {
            _battleProps.Add(battleProp);
            Signals.Emit(new SignalArgs(SignalNames.SKILL_LEARNED, battleProp.gameObject));
        }

        public BattleProp GetBattleProp(UniqueID skillID)
        {
            for (int i = 0; i < _battleProps.Count; i++)
            {
                if (_battleProps[i].SkillData.ID.ToString() == skillID.ToString())
                {
                    return _battleProps[i];
                }
            }

            return null;
        }

        public void OnReceiveSignal(SignalArgs args)
        {
            if (Character.IsDead) return;
            if (args.signal == SignalNames.SPELL_HIT)
            {
                OnSkillHit(args.invoker.Get<BattleProp>());
                return;
            }
        }

        public UpgradeSkillData[] ChoiceSkillDatas()
        {
            return _skillManager.ChoiceSkillDatas();
        }

        public void LearnSkill(string skillID, SkillEffectStatValue skillEffectStatValue)
        {
            _skillManager.LearnSkill(skillID, skillEffectStatValue);
        }

        public void ResetSelf(int id)
        {
            ID = id;
            Character.IsDead = false;
            OnCreate();
        }
    }
}