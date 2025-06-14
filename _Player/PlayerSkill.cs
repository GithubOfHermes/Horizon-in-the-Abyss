using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
	public GameObject skillPrefab;
	public Transform skillPoint;
	private PlayerStat stat;

	void Start()
	{
		stat = GetComponent<PlayerStat>();
	}

	void Update()
	{
		
    }

	public void TryUseSkill()
	{
		if (stat.currentMP >= stat.maxMP)
		{
			stat.currentMP = 0;
			GameObject skill = Instantiate(skillPrefab, skillPoint.position, transform.rotation);
			Destroy(skill, 2f);
		}
	}
	
	void OnTriggerEnter(Collider other)
    {
        // ■ 수정: Enemy만이 아니라 Elite 태그도 처리
        if (other.CompareTag("Enemy") || other.CompareTag("Elite") || other.CompareTag("Boss"))
        {
            // EnemyManager가 있으면 일반 적으로 처리
            var em = other.GetComponent<EnemyManager>();
            if (em != null)
            {
                em.Enemy_TakeDamage(stat.skillDamage);  // ■ 수정: 스킬 데미지 적용
            }
            else
            {
                // EliteManager가 있으면 Elite 대상 처리
                var elite = other.GetComponent<EliteManager>();  // ■ 수정: EliteManager 참조
                if (elite != null)
                    elite.TakeDamage(stat.skillDamage);         // ■ 수정: Elite 데미지 적용
                else
                {
                    // ■ 수정: Boss 처리
                    var bm = other.GetComponent<BossManager>();
                    if (bm != null)
                        bm.TakeDamage(stat.skillDamage);
                }
            }
        }
    }
}

