using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weber.Scripts.Domain;

namespace Weber.Scripts.Legend
{
    public class Login : MonoBehaviour
    {
        private async void Start()
        {
            await HeroManager.Instance.LoadConfig();
            SceneManager.LoadSceneAsync("Game");
        }
    }
}