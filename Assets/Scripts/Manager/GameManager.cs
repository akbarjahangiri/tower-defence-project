using System;
using Enemy;
using UnityEngine;
using Utility.Patterns;
using WaveSystem;

namespace Manager
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] private WaveSystemManager waveSystemManager;

        // base health
        [SerializeField] private int maxAllowedToReachGoal = 3;
        private int _reachedGoalCount;

        [SerializeField] private EnemySpawner enemySpawner;

        public Action OnGameFinished;

        private void Start()
        {
            waveSystemManager.OnWaveIndexChanged += HandleWaveIndexChanged;
        }

        public void Init()
        {
            _reachedGoalCount = 0;
            waveSystemManager.Init(0);
            
            Debug.LogWarning(
                $"[GameManager] Starting Waves — Current Wave: {waveSystemManager.CurrentWaveIndex + 1}, Total Waves: {waveSystemManager.TotalWavesNumber}");


            waveSystemManager.StartWave();
            UIManager.Instance.UpdateWave(waveSystemManager.CurrentWaveIndex + 1, waveSystemManager.TotalWavesNumber);
        }

        private void HandleWaveIndexChanged(int newIndex)
        {
            UIManager.Instance.UpdateWave(waveSystemManager.CurrentWaveIndex + 1, waveSystemManager.TotalWavesNumber);
            Debug.Log($"[GameManager] Wave Changed! Now at Wave {newIndex + 1} / {waveSystemManager.TotalWavesNumber}");
        }

        private void HandleEnemyReachedGoal(BaseEnemy enemy)
        {
            enemy.ReachedGoalEvent -= HandleEnemyReachedGoal;

            _reachedGoalCount += enemy.ScoreValue;
            Debug.Log($"Enemy reached goal. Count = {_reachedGoalCount}/{maxAllowedToReachGoal}");

            if (_reachedGoalCount >= maxAllowedToReachGoal)
            {
                LoseGame();
            }
        }

        public void SpawnEnemy(Transform[] waypoints, EnemyType enemyType)
        {
            var enemy = enemySpawner.GetEnemiesByType(enemyType);
            enemy.SetWaypoints(waypoints);
            enemy.ReachedGoalEvent += HandleEnemyReachedGoal;
        }
        
        private void LoseGame()
        {
            Debug.Log("Game Over! You lost.");
            waveSystemManager.StopWave();
            enemySpawner.DestroyAllEnemies(); // Destroy all active enemies
            UIManager.Instance.HandleLoseGame();
            OnGameFinished?.Invoke();
        }

        private void OnDestroy()
        {
            if (waveSystemManager != null)
            {
                waveSystemManager.OnWaveIndexChanged -= HandleWaveIndexChanged;
            }
        }
    }
}