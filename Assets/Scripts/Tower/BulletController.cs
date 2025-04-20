using DG.Tweening;
using Enemy;
using UnityEngine;

namespace Tower
{
    [RequireComponent(typeof(Collider))]
    public class BulletController : MonoBehaviour
    {
        [Header("Bullet Settings")] [SerializeField]
        private float speed = 15f;

        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private AudioClip hitSound;

        private Tween _moveTween;
        private Tween _lifetimeTween;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        public void Set(Transform target, Vector3 startPosition)
        {
            transform.SetParent(null);
            transform.position = startPosition;

            var targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

            transform.LookAt(targetPosition);

            _moveTween?.Kill();
            _lifetimeTween?.Kill();

            float distance = Vector3.Distance(startPosition, target.position);
            float travelTime = distance / speed;

            _moveTween = transform
                .DOMove(targetPosition, travelTime)
                .SetEase(Ease.Linear)
                .OnComplete(Deactivate);

            _lifetimeTween = DOVirtual.DelayedCall(lifetime, Deactivate, ignoreTimeScale: true);

            _collider.enabled = true;
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_collider.enabled) return;

            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<BaseEnemy>(out var enemy))
                {
                    enemy.TakeDamage(damage);

                    if (hitEffect != null)
                    {
                        var fx = Instantiate(hitEffect, transform.position, Quaternion.identity);
                        Destroy(fx.gameObject, fx.main.duration);
                    }

                    if (hitSound != null)
                        AudioSource.PlayClipAtPoint(hitSound, transform.position);

                    Deactivate();
                }
            }
        }

        private void Deactivate()
        {
            _collider.enabled = false;
            _moveTween?.Kill();
            _lifetimeTween?.Kill();
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _moveTween?.Kill();
            _lifetimeTween?.Kill();
        }
    }
}