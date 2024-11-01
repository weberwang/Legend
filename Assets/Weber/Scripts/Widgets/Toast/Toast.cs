using System;
using DG.Tweening;
using GameCreator.Runtime.Common;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Weber.Widgets.Toast
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Toast : MonoBehaviour
    {
        [SerializeField] private float duration = 2;
        [SerializeField] private TextMeshProUGUI messageText;
        private const string PrefabPath = "Assets/Weber/Prefabs/Toast.prefab";

        public async static void Show(string message)
        {
            var operationHandle = Addressables.LoadAssetAsync<GameObject>(PrefabPath);
            await operationHandle.Task;
            var toastGameObject = Instantiate(operationHandle.Result);
            toastGameObject.Get<RectTransform>().SetParent(ToastCanvas.Instance.transform, false);
            var toast = toastGameObject.Get<Toast>();
            toast.Initialize(message);
        }

        private void Start()
        {
            var canvasGroup = gameObject.Require<CanvasGroup>();
            canvasGroup.DOFade(0, duration - 1).SetDelay(1).SetEase(Ease.OutQuad);
            transform.localPosition = new Vector3(0, -200, 0);
            transform.DOLocalMoveY(500, duration).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() => { Destroy(gameObject); });
        }

        private void Initialize(string message)
        {
            messageText.text = message;
        }
    }
}