using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]

public class BearController : MonoBehaviour
{
    public enum BearState
    {
        Patrol,
        Chase,
        Attack
    }

    [SerializeField] private NavMeshAgent _agent;
    private float _moveSpeed = 3.0f;
    private float _chaseSpeed = 6.0f;
    private float _patrolRadius = 20.0f;
    private float _detectRadius = 6.0f;
    private float _attackRadius = 2.5f;
    private float _pushForce = 12.0f;
    private float _minWaitTime = 1.0f;
    private float _maxWaitTime = 3.0f;
    private float _attackCooldown = 1.7f;
    private float _viewAngle = 90f;
    private float _patrolRotationSpeed = 120.0f;
    private float _chaseRotationSpeed = 360.0f;
    private float _attackHitBuffer = 1.5f;

    private WaitForSeconds _attackDelayWait;
    private Vector3 _spawnedPosition;
    private Vector3 _targetPosition;
    private float _waitTimer;
    private float _cooldownTimer;
    private bool _isWaiting;
    private bool _hasAttackedInThisCycle = false;

    private Rigidbody _rigidbodyComponent;
    private Transform _playerTransform;
    private BearState _currentState = BearState.Patrol;
    private BearAnimationController _bearAnimation;



    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            _rigidbodyComponent = rb;
            _rigidbodyComponent.freezeRotation = true;
        }

        Animator unityAnimator = GetComponentInChildren<Animator>();
        _bearAnimation = new BearAnimationController(unityAnimator);

        _attackDelayWait = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        _spawnedPosition = transform.position;
        UpdateNextPatrolPosition();

        _isWaiting = false;
        ChangeState(BearState.Patrol);
    }

    private void Update()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        switch (_currentState)
        {
            case BearState.Patrol:
                HandlePatrolState();
                if (!_isWaiting)
                {
                    MoveTo(_targetPosition, _moveSpeed, _patrolRotationSpeed);
                }
                break;
            case BearState.Chase:
                HandleChaseState();
                if (_playerTransform != null)
                {
                    MoveTo(_playerTransform.position, _chaseSpeed, _chaseRotationSpeed);
                }
                break;
            case BearState.Attack:
                HandleAttackState();
                _agent.ResetPath();
                if (_playerTransform != null)
                {
                    RotateToTarget(_playerTransform.position, 10f); 
                }
                break;
        }
        Animate();
    }

    //private void FixedUpdate()
    //{
    //    switch (_currentState)
    //    {
    //        case BearState.Patrol:
    //            if (!_isWaiting)
    //            {
    //                MoveTo(_targetPosition, _moveSpeed, _patrolRotationSpeed);
    //            }
    //            break;

    //        case BearState.Chase:
    //            if (_playerTransform != null)
    //            {
    //                MoveTo(_playerTransform.position, _chaseSpeed, _chaseRotationSpeed);
    //            }
    //            break;

    //        case BearState.Attack:
    //            _rigidbodyComponent.linearVelocity = Vector3.zero;
    //            _rigidbodyComponent.angularVelocity = Vector3.zero;

    //            if (_playerTransform != null)
    //            {
    //                RotateToTarget(_playerTransform.position, _chaseRotationSpeed);
    //            }
    //            break;
    //    }
    //}

    private void HandlePatrolState()
    {
        if (ScanForPlayer())
        {
            ChangeState(BearState.Chase);
            return;
        }

        Patrol();
    }

    private void HandleChaseState()
    {
        if (!ScanForPlayer() || _playerTransform == null)
        {
            ChangeState(BearState.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= _attackRadius && _cooldownTimer <= 0)
        {
            ChangeState(BearState.Attack);
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

    private void ChangeState(BearState newState)
    {
        if (_currentState == newState && newState != BearState.Attack) return;

        _currentState = newState;

        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }

    private void Animate()
    {
        if (_bearAnimation == null) return;

        Vector3 horizontalVelocity = new Vector3(_agent.velocity.x, 0, _agent.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        bool isMoving = (currentSpeed > 0.1f);

        bool isWalking = (_currentState == BearState.Patrol && isMoving && !_isWaiting);
        bool isChasing = (_currentState == BearState.Chase && isMoving);
        bool isAttacking = (_currentState == BearState.Attack);

        _bearAnimation.PlayMove(isWalking);
        _bearAnimation.PlayChase(isChasing);
        _bearAnimation.PlayAttack(isAttacking);
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
        yield return _attackDelayWait;

        if (_playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= _attackRadius + _attackHitBuffer)
            {
                KnockbackTarget(_playerTransform);
            }
        }

        _hasAttackedInThisCycle = false;

        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
        }

        if (_playerTransform != null && ScanForPlayer())
        {
            ChangeState(BearState.Chase);
        }
        else
        {
            _isWaiting = false;
            UpdateNextPatrolPosition();
            ChangeState(BearState.Patrol);
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
                UpdateNextPatrolPosition();
            }
            return;
        }


        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
        {
            _isWaiting = true;
            _waitTimer = Random.Range(_minWaitTime, _maxWaitTime);
            _agent.ResetPath();

            
        }
    }

    //private void MoveTo(Vector3 targetPosition, float speed, float rotationSpeed)
    //{
    //    Vector3 direction = (targetPosition - transform.position).normalized;
    //    direction.y = 0;

    //    Vector3 velocity = direction * speed;
    //    velocity.y = _rigidbodyComponent.linearVelocity.y;

    //    RotateToTarget(targetPosition, rotationSpeed);
    //    _rigidbodyComponent.linearVelocity = velocity;
    //}


    private void MoveTo(Vector3 targetPosition, float speed, float rotationSpeed)
    {
        _agent.destination = targetPosition;
        _agent.speed = speed;
        _agent.acceleration = speed * 1.5f;
        _agent.angularSpeed = rotationSpeed;

        _agent.destination = targetPosition;
    }

    private void RotateToTarget(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void UpdateNextPatrolPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _patrolRadius;
        Vector3 randomTarget = new Vector3(
            _spawnedPosition.x + randomCircle.x,
            _spawnedPosition.y,
            _spawnedPosition.z + randomCircle.y
        );

        if (NavMesh.SamplePosition(randomTarget, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
        {
            _targetPosition = hit.position;
        }
        else
        {
            _targetPosition = randomTarget;
        }
    }
}