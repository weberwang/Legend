using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    [RequireComponent(typeof(Traits))]
    public class BattleProp : MonoBehaviour
    {
        [SerializeField] private GameObject _renderObject;
        [SerializeField] protected BindType _bindType;

        [SerializeField, ShowIf("IsBindFollow")]
        protected float _followSpeed;

        [SerializeField, ShowIf("IsBindBone")] protected HandleField _handleField = new HandleField();

        protected Vector3 _offset;

        public CharacterUnit CharacterUnit { get; private set; }
        public AttackSkillData SkillData { get; private set; }


        private Transform _transform;
        private Transform _targetTransform;

        private Traits _traits;

        private CountDown _countDown;

        private RuntimeStatData _cooldownStats;

        private void Awake()
        {
            _transform = gameObject.transform;
            _traits = gameObject.Get<Traits>();
        }


        public virtual void SetUnitTarget(CharacterUnit characterUnit, SkillData skillData)
        {
            CharacterUnit = characterUnit;
            SkillData = skillData as AttackSkillData;
            _targetTransform = CharacterUnit.transform;
            if (_bindType == BindType.Bone)
            {
                var handle = _handleField.Get(new Args(characterUnit.gameObject));
                characterUnit.Character.Props.AttachInstance(handle.Bone, gameObject, handle.LocalPosition, handle.LocalRotation);
            }
            else if (_bindType == BindType.Parent)
            {
                transform.SetParent(characterUnit.transform);
            }

            transform.localPosition = _offset;

            for (int i = 0; i < skillData.stats.Length; i++)
            {
                switch (skillData.stats[i].stat.ID.String)
                {
                    case Constants.TRAITS_COOLDOWN:
                        _cooldownStats = _traits.RuntimeStats.Get(Constants.TRAITS_COOLDOWN);
                        _cooldownStats.Base = skillData.stats[i].value;
                        break;
                }
            }

            _countDown = SkillData.countDown;
            _countDown.UpdateCooldown(_cooldownStats.Value);
            _countDown.Start();
        }


        private void Update()
        {
            OnUpdate();
        }

        private void LateUpdate()
        {
            if (_bindType == BindType.Follow && _followSpeed > 0 && CharacterUnit != null)
            {
                Vector3 targetPosition = _targetTransform.position;
                Vector3 currentPosition = _transform.position;

                // 仅更新x和z坐标
                Vector3 newPosition = Vector3.Lerp(
                    new Vector3(currentPosition.x, _transform.position.y, currentPosition.z),
                    new Vector3(targetPosition.x, _transform.position.y, targetPosition.z),
                    _followSpeed * Time.deltaTime
                );

                transform.position = new Vector3(newPosition.x, currentPosition.y, newPosition.z);
            }
        }

        protected virtual void OnUpdate()
        {
            if (_countDown.OnUpdate())
            {
                OnActive();
            }

            if (_countDown.Ended)
            {
                OnDeactive();
            }
        }

        protected virtual void OnActive()
        {
            if (_renderObject != null) _renderObject.SetActive(true);
        }

        protected virtual void OnDeactive()
        {
            if (_renderObject != null) _renderObject.SetActive(false);
        }

        #region 编辑器

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

        public void UpdateSkill(LearnSkill learnSkill)
        {
            var runtimeAttributeData = _traits.RuntimeAttributes.Get(learnSkill.id);
            runtimeAttributeData.Value += SkillData.GetChangeValue(learnSkill);
        }

        public float GetStat(string statID)
        {
            return Convert.ToSingle(_traits.RuntimeStats.Get(statID).Value + CharacterUnit.GetStat(statID));
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