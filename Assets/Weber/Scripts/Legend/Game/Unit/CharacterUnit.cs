using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Shooter;
using GameCreator.Runtime.Stats;
using UnityEngine;
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
        [SerializeField] private UniqueID _id = new UniqueID(UniqueID.GenerateID());
        [SerializeField] private LayerMask _deathLayer;
        [SerializeField] private SkillManager _skillManager;

        [SerializeField] private AnimationClip _hitClip;
        [SerializeField] private AnimationClip _deathClip;
        public Character Character { get; private set; }

        protected Transform _transform;

        public UniqueID ID => _id;

        protected Traits traits;
        public RuntimeAttributeData HealthAttributes { get; private set; }

        private List<BattleProp> _battleProps = new List<BattleProp>();

        private List<BaseSkillHitEffect> _skillHitEffects = new List<BaseSkillHitEffect>();

        public float Health => (float)HealthAttributes.Value;

        private void Awake()
        {
            _transform = transform;
            Character = GetComponent<Character>();
            traits = GetComponent<Traits>();
            HealthAttributes = traits.RuntimeAttributes.Get(Constants.TRAITS_HEALTH);
            OnCreate();

            Signals.Subscribe(this, SignalNames.OnSpellHit);
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
            Signals.Unsubscribe(this, SignalNames.OnSpellHit);
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

        public void OnHurt(float damage, bool ignoreArmor = false)
        {
            Debug.Log("OnHurt:" + damage);
            var armor = ignoreArmor ? 0 : GetRuntimeStatDataValue(Constants.TRAITS_ARMOR);
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
            var damage = battleProp.GetRuntimeStatDataValue(Constants.TRAITS_DAMAGE) * Random.Range(0.9f, 1.1f);
            var critical = battleProp.GetRuntimeStatDataValue(Constants.TRAITS_CRITICAL);
            var criticalDamage = battleProp.GetRuntimeStatDataValue(Constants.TRAITS_CRITICAL_DAMAGE);
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
            Character.IsDead = true;
            for (int i = 0; i < _battleProps.Count; i++)
            {
                Destroy(_battleProps[i].gameObject);
            }

            _battleProps.Clear();
            await PlayGesture(_deathClip);
            gameObject.SetActive(false);
        }

        private async void Hit()
        {
            //todo 显示伤害效果
            await PlayGesture(_hitClip);
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

            if (skillEffectStatValue.stat.ID.String == Constants.TRAITS_PICK_DISTANCE)
            {
                GetRuntimeStatData(skillEffectStatValue.stat.ID).AddModifier(ModifierType.Percent, skillEffectStatValue.value);
            }
            else
            {
                GetRuntimeStatData(skillEffectStatValue.stat.ID).AddModifier(ModifierType.Constant, skillEffectStatValue.value);
            }
        }

        public double GetAttribute(string attributeID)
        {
            var attribute = traits.RuntimeAttributes.Get(attributeID);
            if (attribute is not null)
            {
                return attribute.Value;
            }

            return 0;
        }

        public float GetRuntimeStatDataValue(string statID)
        {
            var stat = traits.RuntimeStats.Get(statID);
            if (stat is not null)
            {
                return Convert.ToSingle(stat.Value);
            }

            return 0;
        }

        public RuntimeStatData GetRuntimeStatData(IdString statID)
        {
            return traits.RuntimeStats.Get(statID);
        }

        public RuntimeStatData GetRuntimeStatData(string statID)
        {
            return traits.RuntimeStats.Get(statID);
        }

        public void AddBattleProp(BattleProp battleProp)
        {
            _battleProps.Add(battleProp);
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
            if (args.signal == SignalNames.OnSpellHit)
            {
                OnSkillHit(args.invoker.Get<BattleProp>());
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
    }
}