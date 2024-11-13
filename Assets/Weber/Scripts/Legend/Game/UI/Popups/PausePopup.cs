using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Game.UI.Popups
{
    public class PausePopup : Popup
    {
        protected override void OnClose()
        {
            Game.Instance.ResumeGame();
        }
    }
}