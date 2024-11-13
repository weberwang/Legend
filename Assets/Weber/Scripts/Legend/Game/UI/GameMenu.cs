using System;
using System.Runtime.Serialization;
using Cysharp.Text;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
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
        [SerializeField] private SuperTextMesh _levelText;
        [SerializeField] private Table _levelXPTable;

        private Hero _hero;
        private RuntimeStatData _levelRuntimeStatData;
        private RuntimeStatData _xpRuntimeStatData;

        private void Awake()
        {
            Signals.Subscribe(this, SignalNames.ENEMY_DEATH);
            Signals.Subscribe(this, SignalNames.PICK_COIN);
            Signals.Subscribe(this, SignalNames.PICK_XP);
            Signals.Subscribe(this, SignalNames.LEVEL_UP);
        }

        private void OnDestroy()
        {
            Signals.Unsubscribe(this, SignalNames.ENEMY_DEATH);
            Signals.Unsubscribe(this, SignalNames.PICK_COIN);
            Signals.Unsubscribe(this, SignalNames.PICK_XP);
            Signals.Unsubscribe(this, SignalNames.LEVEL_UP);
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
                _xpSlider.value = _levelXPTable.RatioFromCurrentLevel((int)Math.Floor(_xpRuntimeStatData.Value));

                return;
            }

            if (args.signal == SignalNames.LEVEL_UP)
            {
                using (var builder = ZString.CreateStringBuilder())
                {
                    builder.Append(_levelRuntimeStatData.Value);
                    _levelText.text = builder.ToString();
                }

                return;
            }
        }

        public void StartGame()
        {
            if (_hero == null) _hero = ShortcutPlayer.Get<Hero>();
            if (_levelRuntimeStatData == null) _levelRuntimeStatData = _hero.GetRuntimeStatData(TraitsID.TRAITS_LEVEL);
            if (_xpRuntimeStatData == null) _xpRuntimeStatData = _hero.GetRuntimeStatData(TraitsID.TRAITS_XP);
            _gameTimeText.text = "00:00";
            _killedText.text = "0";
            _coinText.text = "0";
            _levelText.text = "1";
            _xpSlider.value = 0;
        }

        public void GameOver()
        {
        }

        private void OnLevelChange(IdString value, double data)
        {
            Debug.Log("升级!");
            _levelText.text = _levelRuntimeStatData.Value.ToString();
        }

        public void OnClickPause()
        {
            PopupManager.ShowPopup(PopupName.POPUP_PAUSE);
        }
    }
}