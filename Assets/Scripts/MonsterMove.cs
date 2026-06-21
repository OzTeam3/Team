using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public enum MonsterState
    {
        Patrol,
        Chase,
        Attack
    }

    private float moveSpeed = 3.0f;
    private float chaseSpeed = 5.0f;
    private float patrolRadius = 5.0f;
    private float detectRadius = 4.0f;
    private float attackRadius = 2.1f;
    private float pushForce = 12.0f;
    private float minWaitTime = 1.0f;
    private float maxWaitTime = 3.0f;
    private float attackCooldown = 1.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float waitTimer;
    private float cooldownTimer;
    private bool isWaiting = false;
    private bool hasAttackedInThisCycle = false;

    private Rigidbody rigidbodyComponent;
    private Transform playerTransform;
    private MonsterState currentState = MonsterState.Patrol;
    private MonsterAnimationController monsterAnimation;

    public bool IsWaiting => isWaiting;
    public bool IsChasing => currentState == MonsterState.Chase;
    public bool IsAttacking => currentState == MonsterState.Attack;

    

    private void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        monsterAnimation = GetComponent<MonsterAnimationController>();
    }

    private void Start()
    {
        startPosition = transform.position;
        SetNewRandomTarget();
        ChangeState(MonsterState.Patrol);
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case MonsterState.Patrol:
                HandlePatrolState();
                break;
            case MonsterState.Chase:
                HandleChaseState();
                break;
            case MonsterState.Attack:
                HandleAttackState();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case MonsterState.Patrol:
                if (!isWaiting)
                {
                    MoveTo(targetPosition, moveSpeed, 10f);
                }
                break;

            case MonsterState.Chase:
                if (playerTransform != null)
                {
                    MoveTo(playerTransform.position, chaseSpeed, 15f);
                }
                break;

            case MonsterState.Attack:
                rigidbodyComponent.linearVelocity = Vector3.zero;
                rigidbodyComponent.angularVelocity = Vector3.zero;

                if (playerTransform != null)
                {
                    LookAtTarget(playerTransform.position, 15f);
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    private void HandlePatrolState()
    {
        if (ScanForPlayer())
        {
            ChangeState(MonsterState.Chase);
            return;
        }

        Patrol();
    }

    private void HandleChaseState()
    {
        if (!ScanForPlayer() || playerTransform == null)
        {
            ChangeState(MonsterState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRadius && cooldownTimer <= 0)
        {
            ChangeState(MonsterState.Attack);
        }
    }

    private void HandleAttackState()
    {
        if (!hasAttackedInThisCycle && cooldownTimer <= 0)
        {
            hasAttackedInThisCycle = true;
            cooldownTimer = attackCooldown;
            StartCoroutine(AttackRoutine());
        }
    }

    private void ChangeState(MonsterState newState)
    {
        if (currentState == newState && newState != MonsterState.Attack) return;

        currentState = newState;

        if (monsterAnimation != null)
        {
            monsterAnimation.PlayChase(currentState == MonsterState.Chase);
            monsterAnimation.PlayAttack(currentState == MonsterState.Attack);
        }
    }

    private bool ScanForPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                playerTransform = hitCollider.transform;
                return true;
            }
        }
        return false;
    }


    private IEnumerator AttackRoutine()
    {
        hasAttackedInThisCycle = true;
        yield return new WaitForSeconds(0.45f);

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRadius + 1.5f)
            {
                KnockbackTarget(playerTransform);
            }
        }

        hasAttackedInThisCycle = false;

        if (playerTransform != null)
        {
            ChangeState(MonsterState.Chase);
        }
        else
        {
            ChangeState(MonsterState.Patrol);
            SetNewRandomTarget();
        }
    }

    private void KnockbackTarget(Transform target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody != null)
        {
            Vector3 pushDirection = (target.position - transform.position).normalized;
            pushDirection.y = 0.5f;

            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);

            Debug.Log("공격 타이밍 적중! 플레이어를 밀쳐냈습니다.");
        }
    }

    private void Patrol()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewRandomTarget();
            }
            return;
        }

        Vector3 currentPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPosXZ = new Vector3(targetPosition.x, 0, targetPosition.z);

        if (Vector3.Distance(currentPosXZ, targetPosXZ) < 0.2f)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
            rigidbodyComponent.linearVelocity = new Vector3(0, rigidbodyComponent.linearVelocity.y, 0);
        }
    }

    private void MoveTo(Vector3 targetPosition, float speed, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        Vector3 velocity = direction * speed;
        velocity.y = rigidbodyComponent.linearVelocity.y;
        rigidbodyComponent.linearVelocity = velocity;

        LookAtTarget(targetPosition, rotationSpeed);
    }

    private void LookAtTarget(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void SetNewRandomTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        targetPosition = new Vector3(startPosition.x + randomCircle.x, startPosition.y, startPosition.z + randomCircle.y);
    }

   
}