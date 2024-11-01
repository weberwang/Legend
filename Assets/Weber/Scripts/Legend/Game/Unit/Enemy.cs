using GameCreator.Runtime.Common;
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

        protected override void OnCreate()
        {
            _characterData = EnemyFactory.Instance.GetEnemyData(ID);
            Character.Motion.Radius = _characterData.radius;
        }

        private void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(Transform.position, Character.Motion.Radius, Vector3.down, _hits, 1, _hitLayerMask);
            if (count > 0)
            {
                Attack(_hits[0].collider.Get<CharacterUnit>());
            }
        }

        private void Attack(CharacterUnit characterUnit)
        {
            // Debug.Log("攻击英雄：" + characterUnit.CharacterData.name);
        }
    }
}