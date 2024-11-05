using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Game.UI;
using Weber.Scripts.Legend.Unit;
using Weber.Widgets;
using Weber.Widgets.Popup;

namespace Weber.Scripts.Legend.Game
{
    public class Game : SingletonMonoBehaviour<Game>
    {
        [SerializeField] private GameMenu _gameMenu;

        public GameState GameState { get; private set; }

        public float GameTime { get; private set; }

        public int PickedCoin { get; private set; }

        public bool Paused { get; private set; }

        private Hero _hero;

        protected virtual void OnCreate()
        {
            Application.targetFrameRate = 300;
            GameState = GameState.Wait;
        }

        private async void Start()
        {
            await HeroManager.Instance.LoadConfig();
            var heroPrefab = await HeroManager.Instance.LoadHeroPrefab("Knight");
            Instantiate(heroPrefab, new Vector3(10, 1, 0), Quaternion.identity);
            StartGame();
            _hero = ShortcutPlayer.Get<Hero>();
            _hero.OnDeath += OnPlayerDeath;
        }


        private void Update()
        {
            switch (GameState)
            {
                case GameState.Playing:
                    if (!Paused)
                    {
                        GameTime += Time.deltaTime;
                        _gameMenu.UpdateGameTime(Convert.ToInt32(GameTime));
                    }

                    break;
            }
        }

        private void OnDestroy()
        {
            if (_hero is not null)
            {
                _hero.OnDeath -= OnPlayerDeath;
            }
        }

        public async void StartGame()
        {
            await LeveManager.Instance.StartGame(1);
            GameState = GameState.Playing;
            GameTime = 0;
            PickedCoin = 0;
            _gameMenu.StartGame();
        }

        public void PauseGame()
        {
            Paused = true;
            TimeManager.Instance.SetTimeScale(0, 0);
            Signals.Emit(new SignalArgs(SignalNames.PAUSE_GAME, null));
        }

        public void ResumeGame()
        {
            Paused = false;
            TimeManager.Instance.SetTimeScale(1, 0);
        }

        public void EndGame()
        {
            GameState = GameState.Over;
        }

        public void PickCoin(int coin)
        {
            PickedCoin += coin;
            Signals.Emit(new SignalArgs(SignalNames.PICK_COIN, gameObject));
        }

        private void OnPlayerDeath(CharacterUnit arg0)
        {
            EndGame();
        }
    }

    public enum GameState
    {
        Wait,
        Playing,
        Over
    }
}