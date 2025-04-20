using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Manager;
using Poorya.Scripts.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace WaveSystem
{
    [Serializable]
    public class WaveSystemManager : IWave
    {
        [SerializeField] private WaveConfig[] waveConfig;
        [SerializeField] private Vector2 timeBetweenWaves = new Vector2(1f, 1.5f);
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private int totalWavesNumber = 1;

        private int _waveIndex;
        private CancellationTokenSource _cts;

        private WaveConfig CurrentWave => waveConfig[_waveIndex];

        public int TotalWavesNumber => waveConfig.Length;
        public int CurrentWaveIndex => _waveIndex;
        public Action<int> OnWaveIndexChanged;

        public void Init(int waveIndex)
        {
            _waveIndex = waveIndex;
        }

        public void StartWave()
        {
            StopWave();
            _cts = new CancellationTokenSource();
            RunWaveAsync(_cts.Token).Forget();
        }

        public void StopWave()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private async UniTaskVoid RunWaveAsync(CancellationToken token)
        {
            try
            {
                while (_waveIndex < TotalWavesNumber)
                {
                    var wave = CurrentWave;
                    OnWaveIndexChanged?.Invoke(_waveIndex);

                    int spawnCount = UnityEngine.Random.Range(
                        wave.enemiesPerWave.x,
                        wave.enemiesPerWave.y + 1
                    );
                    Debug.Log($"[Wave {totalWavesNumber}] Spawning {spawnCount} enemies.");

                    for (int i = 0; i < spawnCount; i++)
                    {
                        GameManager.Instance.SpawnEnemy(waypoints, wave.enemyTypes);

                        float delay = UnityEngine.Random.Range(
                            wave.timeBetweenEnemies.x,
                            wave.timeBetweenEnemies.y
                        );
                        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
                    }

                    Debug.Log($"[Wave {totalWavesNumber}] Complete. Waiting for next wave...");

                    float waveDelay = UnityEngine.Random.Range(
                        timeBetweenWaves.x,
                        timeBetweenWaves.y
                    );
                    await UniTask.Delay(TimeSpan.FromSeconds(waveDelay), cancellationToken: token);
                    Debug.Log($"[Wave {_waveIndex}] ");

                    _waveIndex = (_waveIndex + 1);
                    Debug.Log($"[Wave end {_waveIndex}] ");

                    if (_waveIndex == totalWavesNumber)
                    {
                        Debug.Log("[Wave {totalWavesNumber}] Ending.");
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[WaveSystem] Wave loop canceled.");
            }
        }
    }
}