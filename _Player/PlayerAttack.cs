using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStat stat;
    private bool hasHit = false; 

    void Start()
    {
        stat = GetComponentInParent<PlayerStat>();
    }

    public void ResetHitFlag()
    {
        hasHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // 이미 데미지 처리했으면 무시

        // ■ 수정: Enemy만 처리하던 기존 코드 → Enemy 또는 Elite 태그 모두 처리
        // 기존: if (other.CompareTag("Enemy"))
        if (other.CompareTag("Enemy") || other.CompareTag("Elite") || other.CompareTag("Boss"))
        {
            hasHit = true; // 첫 충돌 시 플래그 설정

            // EnemyManager가 있으면 데미지, 없으면 EliteManager로 처리
            var em = other.GetComponent<EnemyManager>();
            if (em != null)
                em.Enemy_TakeDamage(stat.attackDamage);
            else
            {
                var elite = other.GetComponent<EliteManager>();  // ■ 수정: EliteManager 참조
                if (elite != null)
                    elite.TakeDamage(stat.attackDamage);
                else
                {
                    var bm = other.GetComponent<BossManager>();
                    if (bm != null)
                        bm.TakeDamage(stat.attackDamage);
                }
            }
        }
    }
}
