using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Game.UI.Popups
{
    public class PausePopup : Popup
    {
        private void OnDestroy()
        {
            Game.Instance.ResumeGame();
        }
    }
}