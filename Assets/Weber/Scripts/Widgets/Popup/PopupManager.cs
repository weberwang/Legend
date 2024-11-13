using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Weber.Widgets.Popup
{
    public class PopupManager
    {
        private static List<Popup> _popups = new List<Popup>();

        private const string PopupPath = "Assets/Weber/Addressable/Prefabs/Popups/{0}.prefab";

        public async static UniTask ShowPopup(string name, object data = null, UnityAction onClose = null)
        {
            // Show popup
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(string.Format(PopupPath, name));
            await asyncOperationHandle.Task;
            var result = asyncOperationHandle.Result;
            var popupGameObject = Object.Instantiate(result);
            popupGameObject.transform.SetParent(PopupCanvas.Instance.transform);
            popupGameObject.transform.localPosition = Vector3.zero;
            popupGameObject.transform.localScale = Vector3.one;
            var popup = popupGameObject.Get<Popup>();
            popup.popupName = name;
            popup.Show(data);
            popup.EventClose += onClose;
            _popups.Add(popup);
        }

        public static void HidePopup(string name)
        {
            // Hide popup
            foreach (var popup in _popups)
            {
                if (popup.popupName == name)
                {
                    popup.Hide();
                    _popups.Remove(popup);
                    return;
                }
            }
        }

        public static void Clear()
        {
            foreach (var popup in _popups)
            {
                popup.Hide();
            }

            _popups.Clear();
        }

        public static bool Exist(string name)
        {
            foreach (var popup in _popups)
            {
                if (popup.popupName == name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}