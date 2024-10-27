using System;
using UnityEngine;

namespace Weber.Scripts.Legend.Skill
{
    public class SpinBullet : SpellBullet
    {
        [SerializeField] private LayerMask _obstacleLayer;

        private bool _changeSpin = false;

        protected override void OnStart()
        {
            pierce = true;
            mode = SpellShootProp.SpellBulletMode.UseTracer;
        }

        private void OnEnable()
        {
            _changeSpin = false;
        }

        protected override void BeyondDistance()
        {
            if (!_changeSpin)
            {
                startTime = Time.time;
                lastMuzzlePosition = transform.position;
                tracerTarget = source.CharacterUnit.transform;
                _changeSpin = true;
            }

            if (Vector3.Distance(tracerTarget.position, transform.position) < 0.1f)
            {
                //回到目标位置
                gameObject.SetActive(false);
            }
        }

        protected override void OnHit(RaycastHit hit)
        {
            base.OnHit(hit);
            if ((_obstacleLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                //立刻返回
                isBeyondDistance = true;
                lastMuzzlePosition = transform.position;
                //飞回到发射点
                tracerTarget = source.CharacterUnit.transform;
                startTime = Time.time;
            }
        }
    }
}