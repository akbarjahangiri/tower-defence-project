using UnityEngine;
using Utility.Helpers;

namespace Enemy
{
    public class EnemyController : BaseEnemy
    {
        [Separator("Death Effects")] 
        [SerializeField] private ParticleSystem deathEffect;

        [SerializeField] private AudioClip deathSound;
        
        public override void ReachGoal()
        {
            Debug.Log($"{EnemyType} enemy has reached the goal.");
            base.ReachGoal();
            gameObject.SetActive(false);
        }

        protected override void Die()
        {
            if (deathEffect != null)
            {
                ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(effect.gameObject, effect.main.duration);
            }

            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            Debug.Log($"{EnemyType} enemy has died. Awarded {scoreValue} points.");

            gameObject.SetActive(false);
        }
    }
}