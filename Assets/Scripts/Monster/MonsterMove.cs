using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public enum MonsterState
    {
        Patrol,
        Chase,
        Attack
    }

    private float _moveSpeed = 3.0f;
    private float _chaseSpeed = 7.0f;
    private float _patrolRadius = 20.0f;
    private float _detectRadius = 6.0f;
    private float _attackRadius = 2.5f;
    private float _pushForce = 12.0f;
    private float _minWaitTime = 1.0f;
    private float _maxWaitTime = 3.0f;
    private float _attackCooldown = 1.7f;
    private float _viewAngle = 90f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _waitTimer;
    private float _cooldownTimer;
    private bool _isWaiting = false;
    private bool _hasAttackedInThisCycle = false;

    private Rigidbody _rigidbodyComponent;
    private Transform _playerTransform;
    private MonsterState _currentState = MonsterState.Patrol;
    private MonsterAnimationController _monsterAnimation;



    private void Awake()
    {
        _rigidbodyComponent = GetComponent<Rigidbody>();
        _monsterAnimation = GetComponent<MonsterAnimationController>();
    }

    private void Start()
    {
        _startPosition = transform.position;
        SetNewRandomTarget();

        _isWaiting = false;

        ChangeState(MonsterState.Patrol);

       
    }

    private void Update()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        switch (_currentState)
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

        Animate();
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case MonsterState.Patrol:
                if (!_isWaiting)
                {
                    MoveTo(_targetPosition, _moveSpeed, 10f);
                }
                break;

            case MonsterState.Chase:
                if (_playerTransform != null)
                {
                    MoveTo(_playerTransform.position, _chaseSpeed, 15f);
                }
                break;

            case MonsterState.Attack:
                _rigidbodyComponent.linearVelocity = Vector3.zero;
                _rigidbodyComponent.angularVelocity = Vector3.zero;

                if (_playerTransform != null)
                {
                    LookAtTarget(_playerTransform.position, 15f);
                }
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? _startPosition : transform.position, _patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
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
        if (!ScanForPlayer() || _playerTransform == null)
        {
            ChangeState(MonsterState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= _attackRadius && _cooldownTimer <= 0)
        {
            ChangeState(MonsterState.Attack);
        }
    }

    private void HandleAttackState()
    {
        if (!_hasAttackedInThisCycle && _cooldownTimer <= 0)
        {
            _hasAttackedInThisCycle = true;
            _cooldownTimer = _attackCooldown;
            StartCoroutine(AttackRoutine());
        }
    }

    private void ChangeState(MonsterState newState)
    {
        if (_currentState == newState && newState != MonsterState.Attack) return;

        _currentState = newState;

      
    }

    private void Animate()
    {
        if (_monsterAnimation == null) return;

        Vector3 horizontalVelocity = new Vector3(_rigidbodyComponent.linearVelocity.x, 0, _rigidbodyComponent.linearVelocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        bool isMoving = (currentSpeed > 0.1f);

        bool isWalking = (_currentState == MonsterState.Patrol && isMoving && !_isWaiting);
        bool isChasing = (_currentState == MonsterState.Chase && isMoving);
        bool isAttacking = (_currentState == MonsterState.Attack);

        _monsterAnimation.PlayMove(isWalking);
        _monsterAnimation.PlayChase(isChasing);
        _monsterAnimation.PlayAttack(isAttacking);
    }

    private bool ScanForPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Player"))
            {
                continue;
            }

            Vector3 dir = (hitCollider.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle < _viewAngle * 0.5f)
            {
                _playerTransform = hitCollider.transform;
                return true;
            }

        }
        return false;
    }


    private IEnumerator AttackRoutine()
    {
        _hasAttackedInThisCycle = true;
        yield return new WaitForSeconds(0.6f);

        if (_playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= _attackRadius + 1.5f)
            {
                KnockbackTarget(_playerTransform);
            }
        }

        _hasAttackedInThisCycle = false;

        if (_playerTransform != null)
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
            targetRigidbody.AddForce(pushDirection * _pushForce, ForceMode.Impulse);

            Debug.Log("공격 적중! 플레이어를 밀쳐냈습니다.");
        }
    }

    private void Patrol()
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
            _rigidbodyComponent.linearVelocity = new Vector3(0, _rigidbodyComponent.linearVelocity.y, 0);

            
        }
    }

    private void MoveTo(Vector3 targetPosition, float speed, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        Vector3 velocity = direction * speed;
        velocity.y = _rigidbodyComponent.linearVelocity.y;
        _rigidbodyComponent.linearVelocity = velocity;

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
        Vector2 randomCircle = Random.insideUnitCircle * _patrolRadius;
        _targetPosition = new Vector3(_startPosition.x + randomCircle.x, _startPosition.y, _startPosition.z + randomCircle.y);
    }


}