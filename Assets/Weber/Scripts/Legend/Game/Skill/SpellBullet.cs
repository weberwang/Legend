using System;
using System.Collections.Generic;
using DG.Tweening;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.Audio;
using GameCreator.Runtime.Melee;
using Sirenix.OdinInspector;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Unit;

namespace Weber.Scripts.Legend.Skill
{
    public class SpellBullet : MonoBehaviour
    {
        protected SpellShootProp.SpellBulletMode mode;
        protected bool pierce = false;
        private Vector3 _tracerDeviation;
        private float _speed;
        private float _lifeTime;
        protected float maxDistance;
        private LayerMask _hitLayer;
        private GameObject _impactEffect;

        private float _impactEffectDuration = 1;

        private AudioClip _impactSound;

        protected float startTime;
        private float _distance;

        private Vector3 _lastPosition;

        private RaycastHit[] RAYCAST_HITS = new RaycastHit[100];

        protected Vector3 velocity = Vector3.zero;

        protected Transform tracerTarget;
        protected SpellShootProp source;
        protected Vector3 lastMuzzlePosition;
        private List<GameObject> _hits = new List<GameObject>();


        private float _impactEffectTime = 0;
        private GameObject _effectInstance;


        private bool _isSphere;
        private float _sphereRadius;
        private float _capsuleHeight;
        private float _capsuleRadius;

        protected bool isBeyondDistance = false;

        private void Start()
        {
            startTime = Time.time;
            _lastPosition = transform.position;
            lastMuzzlePosition = _lastPosition;
            var collider = GetComponent<Collider>();
            collider.enabled = false;
            _isSphere = collider.GetType() == typeof(SphereCollider);
            if (_isSphere)
            {
                _sphereRadius = (collider as SphereCollider).radius;
            }
            else
            {
                var capsuleCollider = GetComponent<CapsuleCollider>();
                _capsuleHeight = capsuleCollider.height;
                _capsuleRadius = capsuleCollider.radius;
            }

            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        private void Update()
        {
            if (_effectInstance is not null)
            {
                _impactEffectTime += Time.deltaTime;
                if (_impactEffectTime >= _impactEffectDuration)
                {
                    if (_effectInstance is not null)
                    {
                        _effectInstance.SetActive(false);
                        _effectInstance = null;
                    }
                }
            }

            if (startTime + _lifeTime <= Time.time)
            {
                gameObject.SetActive(false);
                return;
            }

            Vector3 direction;
            switch (mode)
            {
                case SpellShootProp.SpellBulletMode.UseTracer:
                    if (tracerTarget is null)
                    {
                        direction = UpdateKinematic();
                    }
                    else
                    {
                        direction = UpdateTracer();
                    }

                    break;
                case SpellShootProp.SpellBulletMode.UseKinematic:
                    direction = UpdateKinematic();
                    break;
                default: return;
            }

            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            _distance += Vector3.Distance(transform.position, _lastPosition);
            _lastPosition = transform.position;

            if (_distance >= maxDistance)
            {
                isBeyondDistance = true;
            }

            if (isBeyondDistance)
            {
                BeyondDistance();
            }
        }

        //超过最大距离
        protected virtual void BeyondDistance()
        {
            gameObject.SetActive(false);
        }

        public void SetFromSource(SpellShootProp source, SpellShootProp.SpellBulletMode mode, bool pierce, Vector3 tracerDeviation, float speed, float lifeTime, float maxDistance, LayerMask hitLayer, Vector3 shootDirection, Transform tracerTarget, GameObject impactEffect, float impactEffectDuration, AudioClip impactSound)
        {
            this.source = source;
            this.mode = mode;
            this.pierce = pierce;
            _tracerDeviation = tracerDeviation;
            _speed = speed;
            _lifeTime = lifeTime;
            this.maxDistance = maxDistance;
            _hitLayer = hitLayer;
            this.tracerTarget = tracerTarget;
            _impactEffect = impactEffect;
            _impactEffectDuration = impactEffectDuration;
            _impactSound = impactSound;

            startTime = Time.time;
            _distance = 0;
            _lastPosition = transform.position;
            velocity = shootDirection * _speed;
            _lastPosition = transform.position;
            lastMuzzlePosition = _lastPosition;
            _distance = 0;
            isBeyondDistance = false;
            _hits.Clear();
        }

        protected virtual Vector3 UpdateKinematic()
        {
            Debug.Log("velocity:" + velocity);
            Vector3 deltaTranslation = velocity * Time.deltaTime;
            Vector3 nextPosition = transform.position + deltaTranslation;
            Vector3 nextDirection = nextPosition - transform.position;

            TryRayHit(nextDirection);

            transform.position = nextPosition;
            return velocity.normalized;
        }

        private Vector3 UpdateTracer()
        {
            if (tracerTarget is null) return Vector3.zero;

            float totalDistance = Vector3.Distance(
                tracerTarget.position,
                lastMuzzlePosition
            );

            float elapsedTime = Time.time - startTime;
            float t = _speed * elapsedTime / totalDistance;

            Vector3 nextPosition = Bezier.Get(
                lastMuzzlePosition,
                tracerTarget.position,
                _tracerDeviation,
                Vector3.zero,
                t
            );

            Vector3 nextDirection = nextPosition - transform.position;
            TryRayHit(nextDirection);

            transform.position = nextPosition;

            if (t > 1f - float.Epsilon)
            {
                gameObject.SetActive(false);
            }

            return nextDirection.normalized;
        }

        protected void TryRayHit(Vector3 nextDirection)
        {
            int numHits = 0;
            if (_isSphere)
            {
                numHits = Physics.SphereCastNonAlloc(transform.position, _sphereRadius, nextDirection.normalized, RAYCAST_HITS, nextDirection.magnitude, _hitLayer, QueryTriggerInteraction.Ignore);
            }
            else
            {
                Vector3 p1, p2;
                GetCapsuleColliderEndpoints(GetComponent<CapsuleCollider>(), out p1, out p2);
                numHits = Physics.CapsuleCastNonAlloc(p1, p2, _capsuleRadius, nextDirection.normalized, RAYCAST_HITS, nextDirection.magnitude, _hitLayer, QueryTriggerInteraction.Ignore);
            }

            for (int i = 0; i < numHits; ++i)
            {
                OnHit(RAYCAST_HITS[i]);
            }
        }

        // 计算 CapsuleCollider 的两端点
        private void GetCapsuleColliderEndpoints(CapsuleCollider capsule, out Vector3 point1, out Vector3 point2)
        {
            // 获取 CapsuleCollider 在世界空间的中心位置
            Vector3 center = capsule.transform.TransformPoint(capsule.center);

            // 获取胶囊体的方向
            float height = Mathf.Max(capsule.height / 2 - capsule.radius, 0f); // 有效高度
            Vector3 axis = Vector3.up; // 默认沿着 Y 轴对称

            // 根据胶囊体的方向设置轴
            switch (capsule.direction)
            {
                case 0:
                    axis = capsule.transform.right;
                    break; // X轴方向
                case 1:
                    axis = capsule.transform.up;
                    break; // Y轴方向
                case 2:
                    axis = capsule.transform.forward;
                    break; // Z轴方向
            }

            // 计算两个端点在世界坐标系中的位置
            point1 = center + axis * height;
            point2 = center - axis * height;
        }

        protected virtual void OnHit(RaycastHit hit)
        {
            if (_hits.Count > 0)
            {
                if (!pierce)
                {
                    return;
                }
            }

            if (hit.transform.gameObject == source.CharacterUnit.gameObject) return;
            if (_hits.Contains(hit.transform.gameObject)) return;
            _hits.Add(hit.transform.gameObject);

            if (_impactSound is not null)
            {
                AudioManager.Instance.SoundEffect.Play(_impactSound, AudioConfigSoundEffect.Default, new Args(gameObject));
            }

            if (_impactEffect is not null)
            {
                var effectInstance = PoolManager.Instance.Pick(_impactEffect, 1);
                effectInstance.transform.position = hit.point;
                _effectInstance = effectInstance;
            }

            Signals.Emit(new SignalArgs(SignalNames.SPELL_HIT, source.gameObject));
            if (!pierce)
            {
                gameObject.SetActive(false);
            }
        }
    }
}