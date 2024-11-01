using System;
using System.Runtime.Serialization;
using Cysharp.Text;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Unit;

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
            Signals.Subscribe(this, SignalNames.OnEnemyDeath);
            Signals.Subscribe(this, SignalNames.OnPickCoin);
            Signals.Subscribe(this, SignalNames.OnPickXP);
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
            if (args.signal == SignalNames.OnEnemyDeath)
            {
                using (var builder = ZString.CreateStringBuilder())
                {
                    builder.Append(EnemyFactory.Instance.EnemyKillCount);
                    _killedText.text = builder.ToString();
                }

                return;
            }

            if (args.signal == SignalNames.OnPickCoin)
            {
                using (var builder = ZString.CreateStringBuilder())
                {
                    builder.Append(Game.Instance.PickedCoin);
                    _coinText.text = builder.ToString();
                }

                return;
            }

            if (args.signal == SignalNames.OnPickXP)
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
    }
}