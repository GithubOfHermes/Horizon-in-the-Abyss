using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Transform target;
    public int damage;                                        // 수정: 외부에서 데미지를 설정받도록 변경
    public float speed = 12f;

    public void SetTarget(Transform playerTransform)          // 수정: 추적 대상 설정
    {
        target = playerTransform;
    }

    public void SetDamage(int dmg)                            // 수정: 데미지 설정 메서드 추가
    {
        damage = dmg;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime; // 수정: 플레이어 추적 이동
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))                    // 수정: 플레이어 충돌 시 데미지 적용
        {
            var ps = other.GetComponent<PlayerStat>();       // 수정: PlayerStat 가져오기
            if (ps != null)
            {
                ps.TakeDamage(damage);                       // 수정: 설정된 데미지로 TakeDamage 호출
            }
            Destroy(gameObject);                             // 수정: 충돌 시 파이어볼 즉시 파괴
        }
    }
}