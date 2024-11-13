using System;
using DG.Tweening;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Weber.Widgets.Popup
{
    public class Popup : MonoBehaviour
    {
        public string popupName;
        [SerializeField] protected RectTransform mask;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected RectTransform content;
        [SerializeField] protected bool showMask = true;

        protected RectTransform _rectTransform;

        protected object _data = null;

        [field: NonSerialized] public event UnityAction EventClose;

        // private Tweener _showTween;
        // private Tweener _hideTween;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }

            // content.Require<CanvasGroup>().alpha = 0;
        }

        protected void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.offsetMax = Vector2.one;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchorMin = Vector2.zero;
            mask.offsetMax = Vector2.one;
            mask.offsetMin = Vector2.zero;
            mask.anchorMax = Vector2.one;
            mask.anchorMin = Vector2.zero;
            var image = mask.Get<Image>();
            if (showMask)
            {
                image.color = new Color(0, 0, 0, 180 / 255f);
            }
            else
            {
                image.color = new Color(0, 0, 0, 0);
            }

            OnStart();
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                OnClose();
            }
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnStart()
        {
        }

        public virtual void Show(object data = null)
        {
            // if (_showTween != null && _showTween.IsPlaying()) return;
            _data = data;
            // _showTween = content.Get<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.Linear);
        }

        public void Close()
        {
            PopupManager.HidePopup(popupName);
        }

        public virtual void Hide()
        {
            // if (_hideTween != null && _hideTween.IsPlaying()) return;
            // _showTween.Kill();
            // _hideTween = content.Get<CanvasGroup>().DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });
            Destroy(gameObject);
        }
    }
}