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
			Destroy(skill, 3f);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			// other.GetComponent<Enemy>().TakeDamage(damage); // 적 스크립트에 따라 조정
		}
	}
}

