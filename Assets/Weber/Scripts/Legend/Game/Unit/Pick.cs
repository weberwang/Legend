using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Scripts.Legend.Game;
using Weber.Scripts.Legend.Game.Items;

namespace Weber.Scripts.Legend.Unit
{
    public class Pick : MonoBehaviour
    {
        [SerializeField] private LayerMaskValue _xpLayer;

        private RaycastHit[] _hits = new RaycastHit[100];

        private Transform _transform;

        private Hero _hero;

        private void Start()
        {
            _transform = transform;
            _hero = GetComponent<Hero>();
        }

        private void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(_transform.position + Vector3.up * 10, _hero.PickRadius, Vector3.down, _hits, 0.5f, _xpLayer.Value);
            for (int i = 0; i < count; i++)
            {
                var result = _hits[i];
                var dropItem = result.collider.GetComponent<DropItem>();
                dropItem.PickByHero();
            }
        }
    }
}