using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Unit;

namespace Weber.Scripts.Legend.Skill
{
    public class FallingStarProp : SpellShootProp
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _randomRadius;
        [SerializeField] private float _heightOffset;

        public override void OnActive()
        {
            //在角色半径外找一个最近的敌人
            var enemies = EnemyFactory.Instance.Enemies;
            CharacterUnit target = null;
            float minDistance = float.MaxValue;
            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(CharacterUnit.transform.position, enemy.transform.position);
                if (distance >= _radius && distance < minDistance)
                {
                    minDistance = distance;
                    target = enemy;
                }
            }

            if (target is null)
            {
                var maxDistance = -1f;
                foreach (var enemy in enemies)
                {
                    var distance = Vector3.Distance(CharacterUnit.transform.position, enemy.transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        target = enemy;
                    }
                }
            }

            if (target is null) return;
            CharacterUnit.Character.Combat.Targets.Primary = target.gameObject;
            //将位置位置到目标的正上方一个圆内的随机位置
            var randomPos = target.transform.position + new Vector3(Random.Range(-_randomRadius, _randomRadius), _heightOffset,
                Random.Range(-_randomRadius, _randomRadius));
            transform.position = randomPos;
            base.OnActive();
        }
    }
}