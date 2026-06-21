using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
//    [SerializeField] private float _attackRadius = 2.0f;

//    //메서드
//    private void Update()
//    {
//        if (Keyboard.current.spaceKey.wasPressedThisFrame)
//        {
//            Attack();
//        }
//    }

//    private void Attack()
//    {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRadius);

//        foreach (var hitCollider in hitColliders)
//        {
//            MonsterMove monster = hitCollider.GetComponent<MonsterMove>();
//            if (monster != null)
//            {
//                //monster.OnHitByPlayer();
//                Debug.Log("몬스터 타격!");
//            }
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, _attackRadius);
//    }
}