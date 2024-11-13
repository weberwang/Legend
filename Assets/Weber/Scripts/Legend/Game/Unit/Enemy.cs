using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Unit
{
    public class Enemy : CharacterUnit
    {
        [SerializeField] private LayerMask _hitLayerMask;
        [SerializeField] private int _prediction = 1;
        [SerializeField] private float _hitGap = 0.1f;

        private RaycastHit[] _hits = new RaycastHit[1];

        public EnemyData EnemyData => _characterData as EnemyData;

        protected override void OnCreate()
        {
            _characterData = EnemyFactory.Instance.GetEnemyData(ID);
            Character.Motion.Radius = _characterData.radius;
            UpdateAttributes();
        }

        private void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(Transform.position, Character.Motion.Radius, Vector3.down, _hits, 1, _hitLayerMask);
            if (count > 0)
            {
                Attack(_hits[0].collider.Get<CharacterUnit>());
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

            GetRunTimeAttributeData(TraitsID.TRAITS_HEALTH).Value = GetRuntimeStatData(TraitsID.TRAITS_MAX_HEALTH).Value;
        }

        private void Attack(CharacterUnit characterUnit)
        {
            // Debug.Log("攻击英雄：" + characterUnit.CharacterData.name);
        }
    }
}