using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Unit
{
    public class Enemy : CharacterUnit
    {
        private EnemyData _enemyData;

        public UnityAction<EnemyData> OnDeath;

        protected override void OnCreate()
        {
        }

        protected override void Death()
        {
            OnDeath?.Invoke(_enemyData);
            base.Death();
        }
    }
}