using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Game.Items
{
    public class DropItem : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _maxSpeed = 5f; // 最大吸引速度
        [SerializeField] private float _speedIncreaseRate = 3f; // 吸引加速度

        // [SerializeField] private Vector3 _tracerDeviation = new Vector3(0, 0, 2);
        [SerializeField] private float _minDistance = 0.1f;
        private Vector3 _lastPosition;
        private bool _startPick = true;
        protected bool tracking = false;

        private float _currentSpeed;

        private Transform _transform;

        protected Hero _hero;

        protected object _value;

        private void Awake()
        {
            _transform = transform;
            _hero = ShortcutPlayer.Get<Hero>();
        }

        private void Update()
        {
            if (tracking)
            {
                if (Vector3.Distance(ShortcutPlayer.Transform.position, _transform.position) <= _minDistance)
                {
                    tracking = false;
                    gameObject.SetActive(false);
                    OnHeroPicked();
                    return;
                }

                float distanceToPlayer = Vector3.Distance(transform.position, _hero.transform.position);
                // 增加吸引速度（以便近距离时更快吸附）
                _currentSpeed = Mathf.Lerp(_speed, _maxSpeed, 1 - (distanceToPlayer / _hero.PickRadius));

                transform.position = Vector3.MoveTowards(transform.position, _hero.transform.position, _currentSpeed * Time.deltaTime);
            }
        }

        public void PickByHero()
        {
            if (_startPick) return;
            _currentSpeed = _speed;
            _startPick = true;
            _lastPosition = transform.position;
            var direction = _hero.transform.position - _transform.position;
            direction.y = 0;
            _transform.DOMove(-direction.normalized * 2, 0.5f).SetRelative(true).OnComplete(() => { tracking = true; });
        }

        protected virtual void OnHeroPicked()
        {
        }

        public async UniTask InitialValue(object value)
        {
            _startPick = true;
            await UniTask.Delay(1000);
            _startPick = false;
            _value = value;
        }
    }
}