using Enemy;
using UnityEngine;

namespace WaveSystem
{
    [CreateAssetMenu(fileName = "wave-config", menuName = "wave/config")]
    public class WaveConfig : ScriptableObject
    {
        public EnemyType enemyTypes = EnemyType.Type1;
        public Vector2Int enemiesPerWave = new Vector2Int(5, 8);
        public Vector2 timeBetweenEnemies = new Vector2(1, 1.5f);
    }
}