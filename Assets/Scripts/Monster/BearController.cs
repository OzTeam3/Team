using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.AddressableAssets;


public enum BearState
{
    Patrol,
    Chase,
    Attack
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]

public class BearController : MonoBehaviour
{
    private float _moveSpeed;
    private float _chaseSpeed;
    private float _patrolRadius;
    private float _detectRadius;
    private float _attackRadius;
    private float _pushForce;
    private float _minWaitTime;
    private float _maxWaitTime;
    private float _attackCooldown;
    private float _viewAngle;
    private float _patrolRotationSpeed;
    private float _chaseRotationSpeed;
    private float _attackHitBuffer;

    private const float _accelerationMagnification = 1.5f;
    private const float _attackDelayTime = 0.6f;
    private const float _knockbackUpwardForce = 0.5f;
    private const float _attackRotationSpeed = 10f;
    private const float _pathRemainingBuffer = 0.1f;
    private const float _halfRatio = 0.5f;

    private Rigidbody _rigidbodyComponent;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private BearAnimationController _bearAnimation;
    private SectorMeshCreator _sectorMeshCreator;

    private BearState _currentState = BearState.Patrol;
    
    private Vector3 _spawnedPosition;
    private Vector3 _targetPosition;

    private float _waitTimer;
    private float _cooldownTimer;
    private bool _isWaiting;
    private bool _hasAttackedInThisCycle;

    private WaitForSeconds _attackDelayWait;

    private void Awake()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            _rigidbodyComponent = rb;
            _rigidbodyComponent.freezeRotation = true;
        }

        if(TryGetComponent(out NavMeshAgent agent))
        {
            _agent = agent;
        }

        _sectorMeshCreator = GetComponentInChildren<SectorMeshCreator>();

        Animator unityAnimator = GetComponentInChildren<Animator>();
        if(unityAnimator != null)
        {
            _bearAnimation = new BearAnimationController(unityAnimator);
        }

        _spawnedPosition = transform.position;
        UpdateNextPatrolPosition();

        _attackDelayWait = new WaitForSeconds(_attackDelayTime);

        MonsterData monsterData = DataManager.Instance.GetData<MonsterData>("Mob_Bear_001");
        InitializeBear(monsterData);
    }

    public void InitializeBear(MonsterData monsterData)
    {
        _moveSpeed = monsterData.MoveSpeed;
        _chaseSpeed = monsterData.ChaseSpeed;
        _patrolRadius = monsterData.PatrolRadius;
        _detectRadius = monsterData.DetectRadius;
        _attackRadius = monsterData.AttackRadius;
        _pushForce = monsterData.PushForce;
        _minWaitTime = monsterData.MinWaitTime;
        _maxWaitTime = monsterData.MaxWaitTime;
        _attackCooldown = monsterData.AttackCooldown;
        _viewAngle = monsterData.ViewAngle;
        _patrolRotationSpeed = monsterData.PatrolRotationSpeed;
        _chaseRotationSpeed = monsterData.ChaseRotationSpeed;
        _attackHitBuffer = monsterData.AttackHitBuffer;
    }

    private void OnEnable()
    {
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
                    RotateToTarget(_playerTransform.position, _attackRotationSpeed); 
                }
                break;
        }
        UpdateAnimationStates();
    }

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

        if(_sectorMeshCreator != null)
        {
            bool shouldShowRange = (newState == BearState.Patrol);

            _sectorMeshCreator.gameObject.SetActive(shouldShowRange);   
        }

        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }

    private void UpdateAnimationStates()
    {
        if (_bearAnimation == null)
        {
            return;
        }

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
            if (angle < _viewAngle * _halfRatio)
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
        if (!target.TryGetComponent(out Rigidbody targetRigidbody))
        {
            Debug.LogError("[BearController:KnockbackTarget] 플레이어 Rigidbody 컴포넌트가 없습니다.");
            return;
        }

        Vector3 pushDirection = (target.position - transform.position).normalized;
        pushDirection.y = _knockbackUpwardForce;

        targetRigidbody.linearVelocity = Vector3.zero;
        targetRigidbody.AddForce(pushDirection * _pushForce, ForceMode.Impulse);
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


        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + _pathRemainingBuffer)
        {
            _isWaiting = true;
            _waitTimer = Random.Range(_minWaitTime, _maxWaitTime);
            _agent.ResetPath();
        }
    }

    private void MoveTo(Vector3 targetPosition, float speed, float rotationSpeed)
    {
        _agent.speed = speed;
        _agent.acceleration = speed * _accelerationMagnification;
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
       
        Vector3 randomTarget = new Vector3(_spawnedPosition.x + randomCircle.x, _spawnedPosition.y, _spawnedPosition.z + randomCircle.y);
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