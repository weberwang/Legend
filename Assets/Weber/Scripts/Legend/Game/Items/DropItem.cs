using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Game.Items
{
    public class DropItem : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Vector3 _tracerDeviation = new Vector3(0, 0, 2);
        [SerializeField] private float _minDistance = 0.1f;
        private Vector3 _lastPosition;
        private bool _picked = false;
        private float _startTime;

        private Transform _transform;

        protected Hero _hero;

        protected object _value;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            _hero = ShortcutPlayer.Get<Hero>();
        }

        private void Update()
        {
            if (_picked)
            {
                if (Vector3.Distance(ShortcutPlayer.Transform.position, _transform.position) <= _minDistance)
                {
                    gameObject.SetActive(false);
                    OnHeroPicked();
                    return;
                }

                float totalDistance = Vector3.Distance(
                    ShortcutPlayer.Transform.position,
                    _lastPosition
                );

                float elapsedTime = Time.time - _startTime;
                float t = _speed * elapsedTime / totalDistance;

                Vector3 nextPosition = Bezier.Get(
                    _lastPosition,
                    ShortcutPlayer.Transform.position,
                    _tracerDeviation,
                    Vector3.zero,
                    t
                );
                if (t > 1)
                {
                    gameObject.SetActive(false);
                }

                transform.position = nextPosition;
            }
        }

        public void PickByHero()
        {
            _startTime = Time.time;
            _picked = true;
        }

        protected virtual void OnHeroPicked()
        {
        }

        public void InitialValue(object value)
        {
            _value = value;
        }
    }
}