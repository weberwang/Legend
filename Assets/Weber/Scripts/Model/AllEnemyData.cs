using System;
using UnityEngine;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "AllEnemyData", menuName = "Weber/Character/AllEnemyData", order = 0)]
    public class AllEnemyData : ScriptableObject
    {
        public EnemyData[] enemyDatas;
    }

    [Serializable]
    public class EnemyData
    {
        public int id;
        public string name;
        public Sprite icon;
        public string description;
        public EnemyType type;
        public int health;
        public int armor;
        public int damage;
        public int exp;
    }

    [Serializable]
    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }
}