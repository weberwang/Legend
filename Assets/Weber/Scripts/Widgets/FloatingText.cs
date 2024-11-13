using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameCreator.Runtime.Common;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Weber.Widgets
{
    public class FloatingText : MonoBehaviour
    {
        //前缀
        [SerializeField] private bool _prefix;
        [SerializeField] private SuperTextMesh _valueText;
        [SerializeField] private Vector2 _offset = new Vector2(0, 100);
        [SerializeField] private float _duration = 1;
        [SerializeField] private bool _billboard = true;

        private static GameObject _floatingTextPrefab;

        private void Start()
        {
            //加血飘动的动画
            transform.DOMove(_offset, _duration).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() => { Destroy(gameObject); });
            if (_billboard)
            {
                //广告牌
                transform.rotation = Camera.main.transform.rotation;
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void SetValue(float value)
        {
            using (var builder = ZString.CreateStringBuilder())
            {
                builder.Append(value);
                _valueText.text = builder.ToString();
            }
        }
    }
}