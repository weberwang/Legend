using System;
using UnityEngine;

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
        private int _amount;

        private bool _end = false;
        public bool Ended => _end;

        private bool _turnOn = false;

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
            _duration = duration;
            _interval = interval;
            _amount = 0;
            _end = false;
        }

        public CountDown Clone()
        {
            return new CountDown(cooldown, duration, interval, amount);
        }

        public bool OnUpdate()
        {
            if (_end) return false;
            OnUpdateCooldown();
            if (!_turnOn)
            {
                return false;
            }

            if (duration <= 0 && interval <= 0)
            {
                CheckAmount();
                _turnOn = false;
                return true;
            }

            _interval -= Time.deltaTime;
            if (_interval <= 0)
            {
                if (duration <= 0)
                {
                    CheckAmount();
                    _turnOn = false;
                }

                _interval = interval;
                return true;
            }

            if (_duration > 0)
            {
                _duration -= Time.deltaTime;
                if (_duration <= 0)
                {
                    CheckAmount();
                    _turnOn = false;
                    return false;
                }
            }


            return false;
        }

        public void End()
        {
            _end = true;
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
                _duration = duration;
                _interval = interval;
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
                    End();
                }
            }
        }
    }
}