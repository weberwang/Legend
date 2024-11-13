using System;
using DG.Tweening;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace Weber.Scripts.Legend.Skill
{
    [RequireComponent(typeof(DOTweenAnimation))]
    public class PropTween : MonoBehaviour
    {
        private DOTweenAnimation _tweenAnimation;

        private void Awake()
        {
            _tweenAnimation = gameObject.Require<DOTweenAnimation>();
            _tweenAnimation.onComplete.AddListener(OnTweenComplete);
        }

        private void OnEnable()
        {
            _tweenAnimation.RecreateTween();
            _tweenAnimation.DOPlay();
        }

        private void OnDisable()
        {
            _tweenAnimation.DOKill();
        }

        private void OnDestroy()
        {
            if (_tweenAnimation is not null && _tweenAnimation.onComplete is not null) _tweenAnimation.onComplete.RemoveListener(OnTweenComplete);
        }

        private void OnTweenComplete()
        {
            gameObject.SetActive(false);
        }
    }
}