using System.Collections.Generic;
using DG.Tweening;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using Utility.Patterns;

namespace Tower
{
    [RequireComponent(typeof(SphereCollider))]
    public class TowerController : MonoBehaviour
    {
        [Header("Shooting Settings")] [Tooltip("Bullets per second")] [SerializeField]
        private float fireRate = 1f;

        [SerializeField] private float shootRange = 10;

        [Tooltip("Bullet prefab pool")] [SerializeField]
        private Pool<BulletController> bulletPool;

        [FormerlySerializedAs("firePoint")] [Tooltip("Point from which bullets are fired")] [SerializeField]
        private Transform barrelTransform;

        [SerializeField] private Transform firePosition;

        [Header("Aiming")] [Tooltip("How quickly the tower turns toward a new target")] [SerializeField]
        private float rotationSpeed = 5f;

        [Tooltip("Degrees per second when idle")] [SerializeField]
        private float idleRotationSpeed = 20f;

        private readonly List<Transform> _enemiesInRange = new List<Transform>();
        private Transform _currentTarget;
        private float _fireCooldown;


        [SerializeField] private Vector3 barrelRecoil = new Vector3(0f, 0f, 0f);
        [SerializeField] private ParticleSystem muzzleFlash;

        private void Awake()
        {
            var col = GetComponent<SphereCollider>();
            col.isTrigger = true;
        }

        private void Update()
        {
            CleanInvalidEnemies();

            if (_currentTarget != null)
            {
                AimAndFire();
            }
            else
            {
                IdleRotate();
            }
        }


        private void CleanInvalidEnemies()
        {
            _enemiesInRange.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);

            if (_currentTarget == null || !_enemiesInRange.Contains(_currentTarget))
            {
                _currentTarget = _enemiesInRange.Count > 0 ? _enemiesInRange[0] : null;
            }
        }

        private void AimAndFire()
        {
            var fullDir = _currentTarget.position - barrelTransform.position;
            var distance = fullDir.magnitude;

            // Compute horizontal direction
            Vector3 dir = _currentTarget.position - barrelTransform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0f)
            {
                // Smoothly rotate toward target
                Quaternion targetRot = Quaternion.LookRotation(dir);
                barrelTransform.rotation = Quaternion.Slerp(
                    barrelTransform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );

                // Check if we're aimed close enough to fire
                float angle = Quaternion.Angle(barrelTransform.rotation, targetRot);

                // Fire when aimed and cooldown allows
                _fireCooldown -= Time.deltaTime;
                // if (!(distance <= shootRange)) return;
                if (angle < 2f && _fireCooldown <= 0f && distance <= shootRange)
                {
                    Shoot();
                    _fireCooldown = 1f / fireRate;
                }
            }
        }

        private void IdleRotate()
        {
            // Gently spin when there's no target
            barrelTransform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<BaseEnemy>(out var enemy))
                {
                    if (_enemiesInRange.Contains(enemy.transform)) return;

                    _enemiesInRange.Add(enemy.transform);

                    if (_currentTarget == null)
                        _currentTarget = enemy.transform;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<BaseEnemy>(out var enemy))
            {
                _enemiesInRange.Remove(enemy.transform);
                if (_currentTarget == enemy.transform)
                    _currentTarget = _enemiesInRange.Count > 0 ? _enemiesInRange[0] : null;
            }
        }

        private void Shoot()
        {
            var bullet = bulletPool.GetActive;
            if (bullet != null && _currentTarget != null)
            {
                // Pass both target and the world‐space firePoint position
                Vector3 startPos = barrelTransform.localPosition;
                Vector3 recoilOffset = barrelTransform.InverseTransformDirection(barrelTransform.forward * -0.5f);

                barrelTransform.DOLocalMove(startPos + recoilOffset, 0.1f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => {
                        barrelTransform.DOLocalMove(startPos, 0.15f).SetEase(Ease.OutBack);
                    });
                var fx = Instantiate(muzzleFlash, firePosition.position, Quaternion.identity);
                Destroy(fx.gameObject, fx.main.duration);
                bullet.Set(_currentTarget, firePosition.position);
            }
        }
    }
}