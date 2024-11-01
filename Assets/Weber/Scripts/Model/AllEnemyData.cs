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
    public class EnemyData:CharacterData
    {
        public EnemyType type;
    }

    [Serializable]
    public enum EnemyType
    {
        Normal,
        Elite,
        Boss
    }
}