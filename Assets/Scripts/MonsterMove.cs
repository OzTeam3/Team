using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3.0f;
    [SerializeField] private float _chaseSpeed = 5.0f;
    [SerializeField] private float _patrolRadius = 5.0f;
    [SerializeField] private float _detectRadius = 4.0f;
    [SerializeField] private float _attackRadius = 1.6f;  
    [SerializeField] private float _pushForce = 12.0f;   
    [SerializeField] private float _minWaitTime = 1.0f;
    [SerializeField] private float _maxWaitTime = 3.0f;
    [SerializeField] private float _attackCooldown = 1.5f; 

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _waitTimer;
    private float _cooldownTimer;      
    private float _hitDelayTimer;      
    private bool _isWaiting = false;
    private bool _isChasing = false;
    private bool _isAttacking = false;
    private bool _hasAttackedInThisCycle = false; 

    private Rigidbody _rigidbody;
    private Animator _animator;
    private Transform _playerTransform;

    // 아래 세 변수가 현재 코드에서 몬스터 상태를 정하는 역할인데 얘네는 외부에서 볼 필요 없을 것 같아요
    public bool IsWaiting => _isWaiting;
    public bool IsChasing => _isChasing;
    public bool IsAttacking => _isAttacking;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _startPosition = transform.position;
        SetNewRandomTarget();
    }

    private void Update() // 행동에 대한 코드가 파편적으로 흩어져 있어서 얘네를 모아서 정리해야할 것 같아요
    {
        CheckForPlayer();

        if (!_isChasing && !_isAttacking)
        {
            HandlePatrolLogic();
        }

        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (_isAttacking)
        {
            _hitDelayTimer += Time.deltaTime;

            if (!_hasAttackedInThisCycle && _cooldownTimer <= 0)
            {
                Attack();
            }
        }

        UpdateAnimationParameters();
    }

    private void FixedUpdate()
    {
        if (_isAttacking)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            if (_playerTransform != null)
            {
                LookAtTarget(_playerTransform.position, 15f);
            }
        }
        else if (_isChasing)
        {
            if (_playerTransform != null)
            {
                MoveTowardsTarget(_playerTransform.position, _chaseSpeed, 15f);
            }
        }
        else if (!_isWaiting)
        {
            MoveTowardsTarget(_targetPosition, _moveSpeed, 10f);
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

    private void CheckForPlayer() // 현재 Check함수 역할이 플레이어 체크와 판단으로 여러 역할이 있습니다. 메서드를 쪼개보는건?
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectRadius);
        bool playerFound = false;

        // player를 찾는 방법은 여러가지가 있을 수 있는데
        // 1. 지금처럼 overlap을 통해 안에 들어온 오브젝트 중 플레이어를 감지하는 방식
        // 2. 사실 플레이어의 transform을 알고 있는 상태고 distance만 비교하는 방식.
        // 아마 현재 target으로 삼은 애의 transform을 참조하거나 추후 매니저를 통해 불러오게 하면 2번 방식도 쓰고
        // 1번 방식도 시야각이나 다른 개념을 도입하려면 이쪽이 적합할 것 같은데요

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
