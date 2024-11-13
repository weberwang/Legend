using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    [RequireComponent(typeof(Traits))]
    public class BattleProp : MonoBehaviour
    {
        [SerializeField] protected GameObject renderObject;
        [SerializeField] protected BindType _bindType;

        [SerializeField, ShowIf("IsBindFollow")]
        protected float _followSpeed;

        [SerializeField, ShowIf("IsBindFollow")]
        private float _distance;

        [SerializeField, ShowIf("IsBindBone")] protected HandleField _handleField = new HandleField();

        [SerializeField, ShowIf("OffsetEnable")]
        protected Vector3 _offset;

        [SerializeField] private bool update = true;
        [field: NonSerialized] public CharacterUnit CharacterUnit { get; private set; }
        [field: NonSerialized] public AttackSkillData SkillData { get; private set; }


        protected Transform _transform;
        protected Transform _targetTransform;

        private Traits _traits;

        [field: NonSerialized] public CountDown CountDown { get; private set; }

        // private RuntimeStatData _cooldownStats;


        private void Awake()
        {
            _transform = gameObject.transform;
            _traits = gameObject.Get<Traits>();
        }

        private void OnDestroy()
        {
            CountDown.EventCooldown -= OnCooldown;
            CountDown.EventTrigger -= OnActive;
        }


        public void SetUnitTarget(CharacterUnit characterUnit, SkillData skillData)
        {
            CharacterUnit = characterUnit;
            SkillData = skillData as AttackSkillData;
            _targetTransform = CharacterUnit.transform;
            switch (_bindType)
            {
                case BindType.Bone:
                    var handle = _handleField.Get(new Args(characterUnit.gameObject));
                    characterUnit.Character.Props.AttachInstance(handle.Bone, gameObject, handle.LocalPosition, handle.LocalRotation);
                    break;
                case BindType.Follow:
                    // transform.localPosition = _offset;
                    break;
                case BindType.Parent:
                    transform.SetParent(characterUnit.transform);
                    transform.localPosition = _offset;
                    break;
            }


            for (int i = 0; i < skillData.stats.Length; i++)
            {
                var skillEffectStatValue = skillData.stats[i];
                if (skillEffectStatValue.ignoreInitialValue)
                {
                    continue;
                }

                var statID = skillEffectStatValue.stat.ID.String;
                var stat = GetRuntimeStatData(statID);
                stat.Base = skillData.stats[i].value;
                stat.AddModifier(ModifierType.Constant, CharacterUnit.GetRuntimeStatValue(statID));
            }

            CountDown = SkillData.countDown.Clone();
            CountDown.UpdateCooldown(GetRuntimeStatDataValue(TraitsID.TRAITS_COOLDOWN));
            CountDown.Start();
            CountDown.EventCooldown += OnCooldown;
            CountDown.EventTrigger += OnActive;
            CharacterUnit.AddBattleProp(this);
            AfterUpdateSkill();
            OnSetTarget();
        }

        protected virtual void OnSetTarget()
        {
        }

        private void Update()
        {
            if (update)
            {
                OnUpdate();
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void LateUpdate()
        {
            if (_bindType == BindType.Follow && _followSpeed > 0 && CharacterUnit != null)
            {
                Vector3 targetPosition = _targetTransform.position;
                targetPosition.y = 0;
                Vector3 currentPosition = _transform.position;
                currentPosition.y = 0;
                if (Vector3.Distance(targetPosition, currentPosition) > _distance)
                {
                    // 仅更新x和z坐标
                    Vector3 newPosition = Vector3.Lerp(
                        new Vector3(currentPosition.x, CharacterUnit.transform.position.y, currentPosition.z),
                        new Vector3(targetPosition.x, CharacterUnit.transform.position.y, targetPosition.z),
                        _followSpeed * Time.deltaTime
                    );

                    transform.position = newPosition + _offset;
                }
            }
        }

        protected virtual void OnUpdate()
        {
            if (CountDown != null)
            {
                // if (CountDown.OnUpdate())
                // {
                //     OnActive();
                // }
                CountDown.OnUpdate();
                // if (CountDown.Ended)
                // {
                //     OnDeactive();
                // }
            }
        }

        public virtual void OnActive()
        {
            if (renderObject != null) renderObject.SetActive(true);
            // for (var i = 0; i < SkillData.stats.Length; i++)
            // {
            //     var skillStat = SkillData.stats[i];
            //     GetRuntimeStatData(skillStat.stat.ID.String).AddModifier(skillStat.changeValueType, skillStat.value);
            // }
        }

        public virtual void OnDeactive()
        {
            if (renderObject != null) renderObject.SetActive(false);
        }

        public virtual void OnCooldown()
        {
        }

        #region 编辑器

        private bool OffsetEnable()
        {
            return _bindType == BindType.Parent || _bindType == BindType.Follow;
        }

        private bool IsBindParent()
        {
            return _bindType == BindType.Parent;
        }

        private bool IsBindBone()
        {
            return _bindType == BindType.Bone;
        }

        private bool IsBindFollow()
        {
            return _bindType == BindType.Follow;
        }

        private bool IsBindNone()
        {
            return _bindType == BindType.None;
        }

        #endregion

        public virtual bool UpdateSkill(SkillEffectStatValue learnSkill)
        {
            if (learnSkill == null) return false;
            try
            {
                var runtimeAttributeData = _traits.RuntimeStats.Get(learnSkill.stat.ID);
                runtimeAttributeData.AddModifier(learnSkill.changeValueType, learnSkill.value);
                if (learnSkill.stat.ID.String == TraitsID.TRAITS_COOLDOWN)
                {
                    CountDown.UpdateCooldown(GetRuntimeStatDataValue(TraitsID.TRAITS_COOLDOWN));
                }

                AfterUpdateSkill(learnSkill);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("找不到属性：" + learnSkill.stat.ID);
                return false;
            }
        }

        protected virtual void AfterUpdateSkill(SkillEffectStatValue learnSkill = null)
        {
        }

        public float GetRuntimeStatDataValue(string statID)
        {
            //所有的变化都已经施加
            var stat = GetRuntimeStatData(statID);
            if (stat == null) return 0;
            return Convert.ToSingle(stat.Value);
        }

        public RuntimeStatData GetRuntimeStatData(string statID)
        {
            return _traits.RuntimeStats.Get(statID);
        }
    }

    [Serializable]
    public enum BindType
    {
        None,
        Parent,
        Follow,
        Bone
    }
}