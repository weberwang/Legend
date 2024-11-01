using System;
using TheraBytes.BetterUi;
using UnityEngine;
using UnityEngine.UI;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Skill;

namespace Weber.Scripts.Legend.Game.UI
{
    public class SkillCountDown : MonoBehaviour
    {
        [SerializeField] private BetterImage _icon;
        [SerializeField] private Image _progressImage;

        private CountDown _countDown;

        public bool Actived { get; private set; }

        private void Awake()
        {
            Actived = false;
            _icon.sprite = null;
            _progressImage.fillAmount = 0;
        }

        public void Active(BattleProp battleProp)
        {
            _icon.sprite = battleProp.SkillData.icon;
            _countDown = battleProp.CountDown;
            Actived = true;
        }

        private void Update()
        {
            if (_countDown != null)
            {
                _progressImage.fillAmount = _countDown.Progress;
            }
        }
    }
}