using System;
using System.Runtime.Serialization;
using Cysharp.Text;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Unit;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Game.UI
{
    public class GameMenu : MonoBehaviour, ISignalReceiver
    {
        [SerializeField] private SuperTextMesh _killedText;
        [SerializeField] private SuperTextMesh _coinText;
        [SerializeField] private SuperTextMesh _gameTimeText;
        [SerializeField] private Slider _xpSlider;

        private Hero _hero;

        private void Awake()
        {
            Signals.Subscribe(this, SignalNames.ENEMY_DEATH);
            Signals.Subscribe(this, SignalNames.PICK_COIN);
            Signals.Subscribe(this, SignalNames.PICK_XP);
        }

        public void UpdateGameTime(int totalSeconds)
        {
            using (var builder = ZString.CreateStringBuilder())
            {
                int minutes = totalSeconds / 60; // 获取分钟数
                int seconds = totalSeconds % 60; // 获取秒数
                builder.AppendFormat("{0:D2}:{1:D2}", minutes, seconds);
                _gameTimeText.text = builder.ToString();
            }
        }

        public void OnReceiveSignal(SignalArgs args)
        {
            if (args.signal == SignalNames.ENEMY_DEATH)
            {
                using (var builder = ZString.CreateStringBuilder())
                {
                    builder.Append(EnemyFactory.Instance.EnemyKillCount);
                    _killedText.text = builder.ToString();
                }

                return;
            }

            if (args.signal == SignalNames.PICK_COIN)
            {
                using (var builder = ZString.CreateStringBuilder())
                {
                    builder.Append(Game.Instance.PickedCoin);
                    _coinText.text = builder.ToString();
                }

                return;
            }

            if (args.signal == SignalNames.PICK_XP)
            {
                return;
            }
        }

        public void StartGame()
        {
            if (_hero is null) _hero = ShortcutPlayer.Get<Hero>();
            _gameTimeText.text = "00:00";
            _killedText.text = "0";
            _coinText.text = "0";
        }

        public void OnClickPause()
        {
            PopupManager.ShowPopup(PopupName.POPUP_PAUSE);
        }
    }
}