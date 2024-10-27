using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Melee;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Legend.Unit;

namespace Weber.Scripts.Legend.Skill
{
    public class SpellMeleeProp : BattleProp
    {
        [SerializeField] private LayerMask _layerMask = -1;
        [SerializeField] private float _hitGap = 0.2f;
        [SerializeField] private int _prediction = 1;
        [SerializeField] private float _delay = 0;

        [SerializeField] private Transform _section;
        [SerializeReference] private TStrikerShape _shape = new StrikerSphere();

        private Dictionary<int, float> _attackedTargets = new Dictionary<int, float>();

        public override void OnActive()
        {
            base.OnActive();
            StartCoroutine("WaitAttack");
        }

        private IEnumerator WaitAttack()
        {
            yield return new WaitForSeconds(_delay);
            var hits = new List<StrikeOutput>();
            var strikeOutputs = _shape.Collect(_section, _layerMask, _prediction);
            // 更新每个目标的冷却时间
            List<int> targetsToRemove = new List<int>();

            foreach (var target in _attackedTargets.Keys)
            {
                _attackedTargets[target] -= Time.deltaTime;

                // 如果冷却时间结束，将目标移除
                if (_attackedTargets[target] <= 0)
                {
                    targetsToRemove.Add(target);
                }
            }

            foreach (var target in targetsToRemove)
            {
                _attackedTargets.Remove(target);
            }

            foreach (var strikeOutput in strikeOutputs)
            {
                if (hits.Contains(strikeOutput)) continue;
                var target = strikeOutput.GameObject.GetComponent<CharacterUnit>();
                if (target == null || target == CharacterUnit) continue;
                if (_attackedTargets.ContainsKey(strikeOutput.GameObject.GetInstanceID()))
                {
                    continue;
                }

                hits.Add(strikeOutput);
                var id = strikeOutput.GameObject.GetInstanceID();
                _attackedTargets[id] = _hitGap;
                Attack(target);
            }
        }

        public override void OnDeactive()
        {
            base.OnDeactive();
            _attackedTargets.Clear();
            StopAllCoroutines();
        }

        private void Attack(CharacterUnit target)
        {
            target.OnSkillHit(this);
        }

        private void OnDrawGizmosSelected()
        {
            _shape.OnDrawGizmos(_section.transform);
        }
    }
}