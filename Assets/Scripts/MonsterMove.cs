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
                playerFound = true;
                _playerTransform = hitCollider.transform;
                break;
            }
        }

        if (playerFound)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _attackRadius)
            {
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    _isChasing = false;
                    _hitDelayTimer = 0f;
                    _hasAttackedInThisCycle = false;
                }
            }
            else
            {
                _isChasing = true;
                _isAttacking = false;
                _isWaiting = false;
            }
        }
        else
        {
            if (_isChasing || _isAttacking)
            {
                _isChasing = false;
                _isAttacking = false;
                SetNewRandomTarget();
            }
        }
    }

    private void ExecuteAutoAttackHit()
    {
        if (_playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        
        // 지금은 일단 몬스터가 플레이어를 직접 AddForce로 밀어내지만
        // 추후에 플레이어가 좀 더 구체적으로 구현되면 플레이어의 TakeDamage나 KnockBack등을 불러내는걸로 변경하죠
        if (distanceToPlayer <= _attackRadius + 1.5f)
        {
            Rigidbody playerRigidbody = _playerTransform.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 pushDirection = (_playerTransform.position - transform.position).normalized;
                pushDirection.y = 0.5f;

                playerRigidbody.linearVelocity = Vector3.zero; 
                playerRigidbody.AddForce(pushDirection * _pushForce, ForceMode.Impulse);

                Debug.Log("🎯 [자동화 시스템] 공격 타이밍 적중! 플레이어를 밀쳐냈습니다.");
            }
        }

        _hasAttackedInThisCycle = true;
        _cooldownTimer = _attackCooldown;
        _hitDelayTimer = 0f; 
    }

    private void HandlePatrolLogic()
    {
        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0)
            {
                _isWaiting = false;
                SetNewRandomTarget();
            }
            return;
        }

        Vector3 currentPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPosXZ = new Vector3(_targetPosition.x, 0, _targetPosition.z);

        if (Vector3.Distance(currentPosXZ, targetPosXZ) < 0.2f)
        {
            _isWaiting = true;
            _waitTimer = Random.Range(_minWaitTime, _maxWaitTime);
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        }
    }

    private void MoveTowardsTarget(Vector3 targetPosition, float speed, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        Vector3 velocity = direction * speed;
        velocity.y = _rigidbody.linearVelocity.y;
        _rigidbody.linearVelocity = velocity;

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

    private void UpdateAnimationParameters()
    {
        if (_animator == null) return;

        _animator.SetBool("isChasing", _isChasing);
        _animator.SetBool("isAttacking", _isAttacking);

        if (_isAttacking && _hasAttackedInThisCycle && _cooldownTimer <= 0)
        {
            _hasAttackedInThisCycle = false;
            _hitDelayTimer = 0f;
        }
    }

    private void SetNewRandomTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _patrolRadius;
        _targetPosition = new Vector3(_startPosition.x + randomCircle.x, _startPosition.y, _startPosition.z + randomCircle.y);
    }

    private void SetMonsterAI() // 코드를 보니까 PlayerCheck가 동일한 역할을 수행중.
    {
        float distance = 0f;

        if (distance <= _attackRadius)
        {
            //attack
            return;
        }
        else if (distance <= _detectRadius)
        {
            // chase
            return;
        }
        //patrol
    }
    private void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectRadius);
        _playerTransform = null;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                _playerTransform = hitCollider.transform;
                break;
            }
        }
    }
    private void DecideAction()
    {
        if(_playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _attackRadius)
            {
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    _isChasing = false;
                    _hitDelayTimer = 0f;
                    _hasAttackedInThisCycle = false;
                }
            }
            else
            {
                _isChasing = true;
                _isAttacking = false;
                _isWaiting = false;
            }
        }
        else
        {
            if (_isChasing || _isAttacking)
            {
                _isChasing = false;
                _isAttacking = false;
                SetNewRandomTarget();
            }
        }
    }
    private void Attack()
    {
        if (_playerTransform == null) return;
        if (_hasAttackedInThisCycle == true) return;

        StartCoroutine(AttackCycle());

        _cooldownTimer = _attackCooldown;
        _hitDelayTimer = 0f;
    }
    private void PushTarget(Transform target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody != null)
        {
            Vector3 pushDirection = (target.position - transform.position).normalized;
            pushDirection.y = 0.5f;

            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.AddForce(pushDirection * _pushForce, ForceMode.Impulse);

            Debug.Log("공격 타이밍 적중! 플레이어를 밀쳐냈습니다.");
        }
    }
    private IEnumerator AttackCycle()
    {
        _hasAttackedInThisCycle = true;
        yield return new WaitForSeconds(0.45f);
        if (_playerTransform == null)
        {
            yield break;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        if (distanceToPlayer <= _attackRadius + 1.5f)
        {
            PushTarget(_playerTransform);
        }
    }
}
