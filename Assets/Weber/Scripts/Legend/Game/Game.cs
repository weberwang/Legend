using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Unit;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Legend.Game
{
    public class Game : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private async void Start()
        {
            await HeroManager.Instance.LoadConfig();
            var heroPrefab = await HeroManager.Instance.LoadHeroPrefab("Knight");
            Instantiate(heroPrefab, new Vector3(10, 1, 10), Quaternion.identity);
            LeveManager.Instance.EndGame(1);
        }
    }
}