﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weber.Scripts.Legend.Unit;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Domain
{
    public class EnemyFactory : Singleton<EnemyFactory>
    {
        private List<CharacterUnit> m_Enemies = new List<CharacterUnit>();

        public List<CharacterUnit> Enemies => m_Enemies;

        private readonly string EnemyPath = "Assets/Weber/Addressable/Enemies/Enemy.prefab";
        private readonly string EnemyModelPath = "Assets/Weber/Addressable/Enemies/Enemy_{0}.prefab";

        private GameObject _enemyPrefab;


        private Camera _mainCamera;
        private readonly int _spawnDistance = 2;
        private readonly float _groundLevel = 0;
        private readonly float _minDistanceFromPlayer = 3f;
        private int _groundMask;
        private RaycastHit[] _hits = new RaycastHit[1];

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitialize()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            LoadEnemy();
            _groundMask = LayerMask.GetMask("Ground");
            _mainCamera = Camera.main;
        }

        private async void LoadEnemy()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(EnemyPath);
            await asyncOperationHandle.Task;
            _enemyPrefab = asyncOperationHandle.Result;
        }

        public async UniTask<Enemy> CreateEnemy(int enemyID)
        {
            var enemyModelPath = string.Format(EnemyModelPath, enemyID);
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(enemyModelPath);
            await asyncOperationHandle.Task;
            Vector3 targetPosition;
            while (!SpawnEnemyOffScreen(out targetPosition))
            {
                await UniTask.Yield();
            }

            targetPosition.y = 1;
            var enemyInstance = PoolManager.Instance.Pick(_enemyPrefab, targetPosition, Quaternion.identity, 1);
            var enemy = enemyInstance.Get<Enemy>();
            enemy.Character.ChangeModel(asyncOperationHandle.Result, new Character.ChangeOptions());
            m_Enemies.Add(enemy);
            return enemy;
        }

        private bool SpawnEnemyOffScreen(out Vector3 position)
        {
            // 获取屏幕边界的四个角（视口 0-1），并转换为世界坐标
            Vector3 screenBottomLeft = GetGroundPosition(new Vector3(0, 0, _mainCamera.nearClipPlane));
            Vector3 screenBottomRight = GetGroundPosition(new Vector3(1, 0, _mainCamera.nearClipPlane));
            Vector3 screenTopLeft = GetGroundPosition(new Vector3(0, 1, _mainCamera.nearClipPlane));
            Vector3 screenTopRight = GetGroundPosition(new Vector3(1, 1, _mainCamera.nearClipPlane));

            // 扩展边界
            float minX = Mathf.Min(screenBottomLeft.x, screenTopLeft.x) - _spawnDistance;
            float maxX = Mathf.Max(screenBottomRight.x, screenTopRight.x) + _spawnDistance;
            float minZ = Mathf.Min(screenBottomLeft.z, screenBottomRight.z) - _spawnDistance;
            float maxZ = Mathf.Max(screenTopLeft.z, screenTopRight.z) + _spawnDistance;

            // 随机选择一个可用的方向
            int side = Random.Range(0, 4);

            // 生成敌人
            Vector3 spawnPosition = Vector3.zero;
            switch (side)
            {
                case 0: // 左边
                    spawnPosition = new Vector3(minX, _groundLevel, Random.Range(minZ, maxZ));
                    break;
                case 1: // 右边
                    spawnPosition = new Vector3(maxX, _groundLevel, Random.Range(minZ, maxZ));
                    break;
                case 2: // 下边
                    spawnPosition = new Vector3(Random.Range(minX, maxX), _groundLevel, minZ);
                    break;
                case 3: // 上边
                    spawnPosition = new Vector3(Random.Range(minX, maxX), _groundLevel, maxZ);
                    break;
            }

            // 确保与玩家保持一定距离
            if (Vector3.Distance(spawnPosition, ShortcutPlayer.Instance.transform.position) < _minDistanceFromPlayer)
            {
                position = Vector3.zero;
                return false;
            }


            //是否在地面上
            if (Physics.RaycastNonAlloc(spawnPosition + Vector3.up * 20, Vector3.down, _hits, 100, _groundMask) <= 0)
            {
                position = Vector3.zero;
                return false;
            }

            position = spawnPosition;
            return true;
        }

        // 将视口坐标转换为在地面上的世界坐标
        Vector3 GetGroundPosition(Vector3 viewportPosition)
        {
            // 将视口坐标转换为摄像机前方的世界坐标
            Vector3 worldPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);

            // 计算从摄像机到该点的射线
            Ray ray = new Ray(_mainCamera.transform.position, worldPosition - _mainCamera.transform.position);

            // 使用射线投射到地面上 (Y = groundY)
            float distanceToGround = (_mainCamera.transform.position.y - _groundLevel) / ray.direction.y;
            Vector3 groundPosition = ray.origin - ray.direction * distanceToGround;

            return groundPosition;
        }

        public void RemoveEnemy(Enemy enemy)
        {
            m_Enemies.Remove(enemy);
            enemy.gameObject.SetActive(false);
        }

        public void RemoveAll()
        {
            foreach (var enemy in m_Enemies)
            {
                enemy.gameObject.SetActive(false);
            }

            m_Enemies.Clear();
        }

        public void Clear()
        {
            foreach (var enemy in m_Enemies)
            {
                Destroy(enemy.gameObject);
            }

            m_Enemies.Clear();
        }

        public CharacterUnit FindNearestEnemy(Vector3 position)
        {
            if (m_Enemies.Count == 0)
            {
                return null;
            }

            float minDistance = float.MaxValue;
            CharacterUnit nearestEnemy = null;
            foreach (var enemy in m_Enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        //查找范围内血量最低的敌人
        public CharacterUnit FindLowestHealthEnemy(Vector3 position, float range)
        {
            if (m_Enemies.Count == 0)
            {
                return null;
            }

            float minHealth = float.MaxValue;
            CharacterUnit lowestHealthEnemy = null;
            foreach (var enemy in m_Enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance <= range && enemy.Health < minHealth)
                {
                    minHealth = enemy.Health;
                    lowestHealthEnemy = enemy;
                }
            }

            return lowestHealthEnemy;
        }
    }
}