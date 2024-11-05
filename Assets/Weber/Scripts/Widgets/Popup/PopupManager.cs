using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Weber.Widgets.Popup
{
    public class PopupManager
    {
        private static List<Popup> _popups = new List<Popup>();

        private const string PopupPath = "Assets/Weber/Addressable/Prefabs/Popups/{0}.prefab";

        public static void ShowPopup(string name, object data = null)
        {
            // Show popup
            Addressables.LoadAssetAsync<GameObject>(string.Format(PopupPath, name)).Completed += handle =>
            {
                var result = handle.Result;
                var popupGameObject = Object.Instantiate(result);
                popupGameObject.transform.SetParent(PopupCanvas.Instance.transform);
                popupGameObject.transform.localPosition = Vector3.zero;
                popupGameObject.transform.localScale = Vector3.one;
                var popup = popupGameObject.Get<Popup>();
                popup.popupName = name;
                popup.Show(data);
                _popups.Add(popup);
            };
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
    }
}