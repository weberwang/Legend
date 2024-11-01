using System;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Game.UI;
using Weber.Scripts.Legend.Unit;
using Weber.Widgets;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Legend.Game
{
    public class Game : SingletonMonoBehaviour<Game>
    {
        [SerializeField] private GameMenu _gameMenu;

        public GameState GameState { get; private set; }

        public float GameTime { get; private set; }

        public int PickedCoin { get; private set; }

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
        }

        private void Update()
        {
            switch (GameState)
            {
                case GameState.Playing:
                    GameTime += Time.deltaTime;
                    _gameMenu.UpdateGameTime(Convert.ToInt32(GameTime));
                    break;
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

        public void PickCoin(int coin)
        {
            PickedCoin += coin;
            Signals.Emit(new SignalArgs(SignalNames.OnPickCoin, gameObject));
        }
    }

    public enum GameState
    {
        Wait,
        Playing,
        Pause,
        Over
    }
}