using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    public class WhirlwindProp : BattleProp
    {
        [SerializeField] private float _force;
        [SerializeField] private Stat _radius;
        [SerializeField] private LayerMask _hitLayer;

        private RaycastHit[] _raycastHits = new RaycastHit[100];

        private float Radius;

        public override void OnActive()
        {
            base.OnActive();
            var hitCount = Physics.SphereCastNonAlloc(_transform.position, Radius, Vector3.up, _raycastHits, 0, _hitLayer.value);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var hit = _raycastHits[i];
                    var unit = hit.collider.GetComponent<CharacterUnit>();
                    if (unit is null)
                    {
                        continue;
                    }

                    var direction = unit.transform.position - _transform.position;
                    direction.y = 0;
                    unit.Knockback(direction.normalized * _force);
                    unit.OnSkillHit(this);
                }
            }
        }

        protected override void AfterUpdateSkill(SkillEffectStatValue learnSkill = null)
        {
            Radius = GetRuntimeStatDataValue(TraitsID.TRAITS_SPELL_SIZE);
        }
    }
}