using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    public class MultipleProp : BattleProp
    {
        [SerializeField] private BattleProp _attackProp;

        private List<GameObject> _battleProps = new List<GameObject>();

        public override void OnActive()
        {
            base.OnActive();
            var attackInstance = PoolManager.Instance.Pick(_attackProp.gameObject, 1);
            _battleProps.Add(attackInstance);
            var prop = attackInstance.Get<BattleProp>();
            prop.SetUnitTarget(CharacterUnit, SkillData);
            prop.OnActive();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _battleProps.Count; i++)
            {
                if (_battleProps[i] is not null) _battleProps[i].SetActive(false);
            }

            _battleProps.Clear();
        }
    }
}