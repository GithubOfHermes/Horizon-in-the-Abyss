using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCollision : MonoBehaviour
{
    private PlayerStat stat;

    void Start()
    {
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();
    }

    void OnTriggerEnter(Collider other)
    {
        // ========== 수정: 이 스크립트가 붙은 오브젝트가 스킬 프리팹인지 검사 ============
        // 스킬 프리팹의 Inspector에서 Tag를 "Skill"로 설정해주세요.
        if (!CompareTag("PlayerSkill")) 
            return;

        // Enemy, Elite, Boss 태그만 처리
        if (other.CompareTag("Enemy") || other.CompareTag("Elite") || other.CompareTag("Boss"))
        {
            var em = other.GetComponent<EnemyManager>();
            if (em != null)
                em.Enemy_TakeDamage(stat.skillDamage);
            else
            {
                var elite = other.GetComponent<EliteManager>();
                if (elite != null)
                    elite.TakeDamage(stat.skillDamage);
                else
                {
                    var bm = other.GetComponent<BossManager>();
                    if (bm != null)
                        bm.TakeDamage(stat.skillDamage);
                }
            }
        }
    }
}
