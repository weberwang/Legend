using System;
using UnityEngine;
using UnityEngine.Events;

namespace Weber.Scripts.Common.Utils
{
    [Serializable]
    public class CountDown
    {
        [SerializeField] private double cooldown;
        [SerializeField] private double duration;
        [SerializeField] private double interval;
        [SerializeField] private int amount = -1;

        private double _cooldown;

        public double Cooldown => cooldown;
        private double _interval;
        private double _duration;

        public double Duration => duration;
        private int _amount;

        private bool _end = false;
        public bool Ended => _end;

        private bool _turnOn = false;

        public float Progress => (float)(_cooldown / cooldown);

        public event UnityAction EventCooldown;
        public event UnityAction EventTrigger;

        public CountDown()
        {
        }

        public CountDown(double cooldown, double duration, double interval, int amount = 1)
        {
            UpdateTime(cooldown, duration, interval, amount);
        }

        public void UpdateTime(double cooldown, double duration, double interval, int amount = 1)
        {
            this.cooldown = cooldown;
            this.duration = duration;
            this.interval = interval;
            this.amount = amount;
            _cooldown = 0;
            _duration = duration;
            _interval = interval;
            _amount = 0;
        }

        public void UpdateCooldown(double cooldown)
        {
            this.cooldown = cooldown;
            _cooldown = 0;
            _duration = duration;
            _interval = interval;
            _amount = 0;
        }

        public void Start()
        {
            _cooldown = 0;
            _duration = 0;
            _interval = interval;
            _amount = 0;
            _end = false;
            _turnOn = true;
        }

        public CountDown Clone()
        {
            return new CountDown(cooldown, duration, interval, amount);
        }

        public void OnUpdate()
        {
            if (_end) return;
            OnUpdateCooldown();
            if (!_turnOn)
            {
                return;
            }

            //有持续时间
            if (!UpdateDuration())
            {
                //进入冷却
                return;
            }

            //立刻触发
            if (interval <= 0)
            {
                if (_duration > 0)
                {
                    //持续中
                    return;
                }

                //duration也为0时触发一次进入冷却
                CheckAmount();
                _duration = duration;
                EventTrigger?.Invoke();
                if (_duration <= 0)
                {
                    _turnOn = false;
                }

                return;
            }

            UpdateInterval();
        }

        private bool UpdateDuration()
        {
            if (duration > 0 && _duration > 0)
            {
                _duration -= Time.deltaTime;
                if (_duration <= 0)
                {
                    CheckAmount();
                    _duration = duration;
                    _turnOn = false;
                    EventCooldown?.Invoke();
                    return false;
                }
            }

            return true;
        }

        private void UpdateInterval()
        {
            if (interval > 0 && _interval > 0)
            {
                _interval -= Time.deltaTime;
                if (_interval <= 0)
                {
                    CheckAmount();
                    _interval = interval;
                    _duration = duration;
                    EventTrigger?.Invoke();
                }
            }
        }

        public void Pause()
        {
            _end = true;
        }

        public void Resume()
        {
            _end = false;
        }

        private void OnUpdateCooldown()
        {
            if (_cooldown > 0)
            {
                _cooldown -= Time.deltaTime;
            }
            else
            {
                _cooldown = cooldown;
                _interval = interval;
                _duration = 0;
                _turnOn = true;
            }
        }

        private void CheckAmount()
        {
            if (amount > 0)
            {
                _amount++;
                if (_amount >= amount)
                {
                    Pause();
                }
            }
        }
    }
}