using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Shooter;
using GameCreator.Runtime.Stats;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    public class SpellShootProp : BattleProp
    {
        [Serializable]
        public enum TracerTarget
        {
            None,
            Nearest,
            LowestHealth,
        }

        [Serializable]
        public enum SpellBulletMode
        {
            UseTracer,
            UseKinematic,
        }

        [SerializeField] private SpellMuzzle[] _muzzles;
        [SerializeField] private GameObject _bullet;
        [SerializeField] private LayerMaskValue _bulletLayer = new LayerMaskValue();
        [SerializeField] private TracerTarget _tracerTarget;

        [SerializeField] private SpellBulletMode _mode;
        [SerializeField] private bool _pierce = false;
        [SerializeField, ShowIf("IsTracer")] private Vector3 _tracerDeviation = Vector3.zero;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _lifeTime = 5;
        [SerializeField] private Stat _maxDistance;
        [SerializeField] private LayerMask _hitLayer;
        [SerializeField] private GameObject _impactEffect;

        [SerializeField, ShowIf("HasImpactEffect")]
        private float _impactEffectDuration = 1;

        [SerializeField] private AudioClip _impactSound;
        private bool IsTracer => _mode == SpellBulletMode.UseTracer;
        private bool HasImpactEffect => _impactEffect is not null;

        private float MaxDistance;

        public override void OnActive()
        {
            base.OnActive();
            for (int i = 0; i < _muzzles.Length; i++)
            {
                Shot(_muzzles[i]);
            }
        }

        protected override void AfterUpdateSkill(SkillEffectStatValue learnSkill)
        {
            MaxDistance = GetRuntimeStatDataValue(_maxDistance.ID.String);
        }

        private void Shot(SpellMuzzle muzzle)
        {
            CharacterUnit targetCharater = FindTarget();
            Transform target;
            if (targetCharater is null) target = null;
            else target = targetCharater.transform;

            Vector3 forword = Vector3.forward;
            if (target is not null && _tracerTarget != TracerTarget.None)
            {
                forword = (target.transform.position - CharacterUnit.transform.position).normalized;
            }

            var direction = muzzle.GetRotation(transform) * forword;
            var bulletInstance = PoolManager.Instance.Pick(_bullet, muzzle.GetPosition(transform), muzzle.GetRotation(transform), 1);
            bulletInstance.gameObject.layer = _bulletLayer.Value;
            var spellBullet = bulletInstance.Require<SpellBullet>();
            spellBullet.SetFromSource(this, _mode, _pierce, _tracerDeviation, _speed, _lifeTime, MaxDistance, _hitLayer, direction, target,
                _impactEffect, _impactEffectDuration, _impactSound);
        }

        private CharacterUnit FindTarget()
        {
            switch (_tracerTarget)
            {
                case TracerTarget.None:
                    return null;
                case TracerTarget.Nearest:
                    return EnemyFactory.Instance.FindNearestEnemy(CharacterUnit.transform.position);
                case TracerTarget.LowestHealth:
                    return EnemyFactory.Instance.FindLowestHealthEnemy(CharacterUnit.transform.position, MaxDistance);
                default:
                    return null;
            }
        }

#if UNITY_EDITOR
        private static readonly Color COLOR_GIZMOS = new Color(0f, 1f, 1f, 1f);
        private const float INDICATOR_GIZMOS = 0.03f;
        private const float RADIUS_GIZMOS = 0.01f;

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < _muzzles.Length; i++)
            {
                Draw(_muzzles[i]);
            }
        }

        private void Draw(SpellMuzzle muzzle)
        {
            Gizmos.color = COLOR_GIZMOS;
            Matrix4x4 restoreMatrix = Gizmos.matrix;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(
                transform.TransformPoint(muzzle.GetPosition(transform)),
                transform.rotation * muzzle.GetRotation(transform),
                Vector3.one
            );

            Gizmos.matrix = rotationMatrix;

            GizmosExtension.Octahedron(Vector3.zero, Quaternion.identity, RADIUS_GIZMOS, 4);
            GizmosExtension.Circle(Vector3.zero, INDICATOR_GIZMOS, Vector3.forward);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.5f);

            Gizmos.matrix = restoreMatrix;
        }
#endif
    }

    [Serializable]
    public class SpellMuzzle
    {
        [SerializeField] private Vector3 _position = Vector3.forward * 0.25f;
        [SerializeField] private Vector3 _rotation = Vector3.zero;

        public Quaternion GetRotation(Transform transform)
        {
            return TransformUtils.TransformRotation(
                Quaternion.Euler(_rotation),
                transform.position,
                transform.rotation,
                transform.lossyScale
            );
        }

        public Vector3 GetPosition(Transform transform)
        {
            return transform.TransformPoint(_position);
        }
    }
}