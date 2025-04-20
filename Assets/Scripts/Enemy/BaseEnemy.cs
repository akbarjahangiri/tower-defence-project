using System;
using UnityEngine;
using Utility.Helpers;

namespace Enemy
{
    public abstract class BaseEnemy : MonoBehaviour
    {
        [SerializeField] protected EnemyType enemyType;
        [SerializeField] protected float maxHealth = 100f;
        public event Action<BaseEnemy> ReachedGoalEvent;

        [SerializeField] protected EnemyMovement enemyMovement;
        protected float currentHealth;
        public float floatCurrentHealth => currentHealth;

        [Separator("Scoring")] [SerializeField]
        internal int scoreValue = 1;

        public int ScoreValue => scoreValue;
        public virtual BaseEnemy InitEnemy()
        {
            return this;
        }

        protected virtual void OnEnable()
        {
            ResetEnemy();
        }

        public virtual void TakeDamage(float damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public virtual void ReachGoal()
        {
            enemyMovement.ResetPosition();
            ReachedGoalEvent?.Invoke(this);
        }

        protected abstract void Die();

        /// (Optional) Reset enemy state when re-using this instance from a pool.
        public virtual void ResetEnemy()
        {
            currentHealth = maxHealth;
        }

        public void SetWaypoints(Transform[] waypoints)
        {
            if (enemyMovement == null)
            {
                enemyMovement = GetComponent<EnemyMovement>();
            }
            
            enemyMovement.SetWaypoints(waypoints);
        }

        public EnemyType EnemyType => enemyType;
    }
}