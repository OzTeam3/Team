using UnityEngine;

public class MonsterHP : MonoBehaviour
{
    [Header("Monster HP Settings")]
    [SerializeField] private float _maxHP = 50f;     
    private float _currentHP;                        
    private bool _isDead = false;                     

    private Animator _animator;
    private Rigidbody _rigidbody;
    private MonsterMove _monsterMove; 

    public bool IsDead => _isDead;
    public float CurrentHP => _currentHP;
    public float MaxHP => _maxHP;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _monsterMove = GetComponent<MonsterMove>();
    }

    private void Start()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHP -= damage;
        Debug.Log($"[MonsterHP] {damage} 피해 접수! 현재 HP: {_currentHP} / {_maxHP}");

        if (_currentHP <= 0)
        {
            Die();
        }
        else
        {
            if (_animator != null)
            {
                _animator.SetTrigger("takeDamage");
            }
        }
    }

    // 몬스터 사망 로직
    private void Die()
    {
        _isDead = true;
        _currentHP = 0; 
        Debug.Log("[MonsterHP] 몬스터 HP가 0이 되어 사망했습니다.");

        if (_monsterMove != null)
        {
            _monsterMove.enabled = false;
        }

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.isKinematic = true; 
        }

        if (_animator != null)
        {
            _animator.SetTrigger("die");
        }

        Destroy(gameObject, 3.0f);
    }


    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TakeDamage(15f);
        }
    }
}