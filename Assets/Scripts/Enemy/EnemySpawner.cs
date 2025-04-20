using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;

namespace Enemy
{
    [Serializable]
    public class EnemySpawner
    {
        [SerializeField] private Pool<BaseEnemy> type1EnemyPool;
        [SerializeField] private Pool<BaseEnemy> type2EnemyPool;
        [SerializeField] private Pool<BaseEnemy> type3EnemyPool;
        private List<BaseEnemy> activeEnemies = new List<BaseEnemy>();

        public BaseEnemy GetEnemiesByType(EnemyType type)
        {
            BaseEnemy enemy;

            switch (type)
            {
                case EnemyType.Type1:
                    enemy = type1EnemyPool.GetActive.InitEnemy();
                    break;
                case EnemyType.Type2:
                    enemy = type2EnemyPool.GetActive.InitEnemy();
                    break;
                case EnemyType.Type3:
                    enemy = type3EnemyPool.GetActive.InitEnemy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unrecognized enemy type");
            }

            activeEnemies.Add(enemy); // Track the spawned enemy
            return enemy;
        }

        public void DestroyAllEnemies()
        {
            type1EnemyPool.DeactivateItems();
            type2EnemyPool.DeactivateItems();
            type3EnemyPool.DeactivateItems();
        }
    }
}