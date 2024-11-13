using System;
using GameCreator.Runtime.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Widgets;
using Weber.Widgets.Toast;

namespace Weber.Scripts.Legend.Unit
{
    [RequireComponent(typeof(CharacterUnit))]
    public class OnHealth : MonoBehaviour
    {
        [SerializeField] private GameObject _damageFloatingTextPrefab;
        [SerializeField] private GameObject _restoreFloatingTextPrefab;
        [SerializeField] private Vector2 _offset = Vector2.one;
        private CharacterUnit _characterUnit;

        private RectTransform _floatingCanvas;

        private void Awake()
        {
            _characterUnit = GetComponent<CharacterUnit>();
            _floatingCanvas = ToastCanvas.Instance.Get<RectTransform>();
        }

        public void OnHealthChange(int damage)
        {
            Debug.Log("血量改变:" + damage);
            GameObject floatingInstance;
            if (damage < 0)
            {
                floatingInstance = PoolManager.Instance.Pick(_damageFloatingTextPrefab, 1);
            }
            else
            {
                floatingInstance = PoolManager.Instance.Pick(_restoreFloatingTextPrefab, 1);
            }

            var position = transform.position;
            //在x和y上加上随机震动
            position.x += UnityEngine.Random.Range(-_offset.x, _offset.x);
            position.y += UnityEngine.Random.Range(-_offset.y, _offset.y);

            floatingInstance.transform.position = position;

            var floating = floatingInstance.Get<FloatingText>();
            floating.SetValue(Convert.ToSingle(damage));
        }
    }
}