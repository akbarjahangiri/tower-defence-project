using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float rotationSpeed = 10f;

        private Transform[] _waypoints;
        private Coroutine _movementCoroutine;

        private void OnEnable()
        {
            if (_movementCoroutine != null)
                StopCoroutine(_movementCoroutine);

            _movementCoroutine = StartCoroutine(MoveAlongPathCoroutine());
        }

        private void OnDisable()
        {
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }
        }

        private IEnumerator MoveAlongPathCoroutine()
        {
            if (_waypoints == null || _waypoints.Length == 0)
            {
                Debug.LogWarning("EnemyMovement: No waypoints defined!");
                yield break;
            }

            ResetPosition();
            int waypointIndex = 0;
            while (waypointIndex < _waypoints.Length)
            {
                Transform targetWaypoint = _waypoints[waypointIndex];

                while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
                {
                    // 1) Face the target waypoint
                    Vector3 direction = (targetWaypoint.position - transform.position).normalized;
                    if (direction.sqrMagnitude > 0f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            targetRot,
                            rotationSpeed * Time.deltaTime
                        );
                    }

                    // 2) Move forward
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetWaypoint.position,
                        speed * Time.deltaTime
                    );

                    yield return null;
                }

                waypointIndex++;
            }

            ReachGoal();
        }

        private void ReachGoal()
        {
            EnemyController controller = GetComponent<EnemyController>();
            if (controller != null)
                controller.ReachGoal();
            else
            {
                Debug.LogWarning("EnemyMovement: No EnemyController found on the enemy.");
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Assigns a new path and restarts movement.
        /// </summary>
        public void SetWaypoints(Transform[] newWaypoints)
        {
            _waypoints = newWaypoints;
            if (_movementCoroutine != null)
                StopCoroutine(_movementCoroutine);

            _movementCoroutine = StartCoroutine(MoveAlongPathCoroutine());
        }

        /// <summary>
        /// Instantly reset the enemy's position to the first waypoint.
        /// </summary>
        public void ResetPosition()
        {
            if (_waypoints != null && _waypoints.Length > 0)
            {
                // Snap to first waypoint
                transform.position = _waypoints[0].position;

                // Face toward the second waypoint (if it exists)
                if (_waypoints.Length > 1)
                {
                    Vector3 dir = _waypoints[1].position - transform.position;
                    if (dir.sqrMagnitude > 0f)
                    {
                        transform.rotation = Quaternion.LookRotation(dir.normalized);
                    }
                }
            }
        }
    }
}
