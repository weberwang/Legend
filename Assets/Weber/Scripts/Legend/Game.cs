﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Unit;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Legend
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Hero _heroPrefab;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            Instantiate(_heroPrefab, new Vector3(10, 1, 10), Quaternion.identity);
            LoadEnemy();
        }

        private async void LoadEnemy()
        {
            //没隔一段时间生成敌人
            // while (true)
            {
                await UniTask.Delay(Random.Range(500, 1000));
                var enemyInstance = await EnemyFactory.Instance.CreateEnemy(0);
                // RemoveEnemy(enemyInstance);
            }
        }

        private async void RemoveEnemy(Enemy enemy)
        {
            await UniTask.Delay(1000);
            EnemyFactory.Instance.RemoveEnemy(enemy);
        }
    }
}