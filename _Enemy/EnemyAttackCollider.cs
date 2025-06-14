using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    private EnemyStat enemyStat; // 수정: 공격력 읽어올 EnemyStat 참조

    void Start()
    {
        // 수정: 부모 EnemyManager → EnemyStat 컴포넌트 자동 할당
        enemyStat = GetComponentInParent<EnemyStat>();
    }

    void OnTriggerEnter(Collider other)
    {
        // 수정: Player 태그 충돌 시
        if (other.CompareTag("Player"))
        {
            var ps = other.GetComponent<PlayerStat>();            // 수정: PlayerStat 가져오기
            if (ps != null)
            {
                ps.TakeDamage(enemyStat.Enemy_attack);            // 수정: EnemyStat의 공격력 적용 :contentReference[oaicite:0]{index=0} :contentReference[oaicite:1]{index=1}
            }
        }
    }
}
