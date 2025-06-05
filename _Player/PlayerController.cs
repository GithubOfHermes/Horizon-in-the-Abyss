using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    private PlayerStat stat;
    private PlayerSkill skill;
    private float attackTimer = 0f;
    private bool isDead = false;

	[Header("PlayerUI")]
	public Slider hpBar;
	public Slider mpBar;

	void Start()
	{
		this.GetComponent<PlayerController>().enabled = false;
		stat = GetComponent<PlayerStat>();
		skill = GetComponent<PlayerSkill>();
		
		// 초기 HP/MP 바 설정 및 Fill 비활성화 처리
		if (hpBar != null) {
			hpBar.maxValue = stat.maxHP;
			hpBar.value = Mathf.Clamp(stat.currentHP, 0, stat.maxHP);

			if (hpBar.fillRect != null && hpBar.fillRect.gameObject != null)
				hpBar.fillRect.gameObject.SetActive(hpBar.value > 0);
		}
		if (mpBar != null) {
			mpBar.maxValue = stat.maxMP;
			mpBar.value = Mathf.Clamp(stat.currentMP, 0, stat.maxMP);

			if (mpBar.fillRect != null && mpBar.fillRect.gameObject != null)
				mpBar.fillRect.gameObject.SetActive(mpBar.value > 0);
		}
    }

    void Update()
	{
		if (isDead) return;

		// HP/MP 바 UI 업데이트
		if (hpBar != null) {
			hpBar.value = Mathf.Clamp(stat.currentHP, 0, stat.maxHP);

			if (hpBar.fillRect != null && hpBar.fillRect.gameObject != null)
				hpBar.fillRect.gameObject.SetActive(hpBar.value > 0);
		}
		if (mpBar != null) {
			mpBar.value = Mathf.Clamp(stat.currentMP, 0, stat.maxMP);

			if (mpBar.fillRect != null && mpBar.fillRect.gameObject != null)
				mpBar.fillRect.gameObject.SetActive(mpBar.value > 0);
		}

		GameObject target = FindNearestEnemy();
		if (target != null)
		{
			float distance = Vector3.Distance(transform.position, target.transform.position);

			if (distance <= stat.attackRange)
			{
				// 이동 애니메이션 중지
				if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
					animator.Play("Idle");

				attackTimer += Time.deltaTime;

				if (attackTimer >= 1f / stat.attackSpeed)
				{
					attackTimer = 0f;

					if (stat.currentMP >= stat.maxMP)
						skill.TryUseSkill();
					else
						StartCoroutine(Attack(target));
				}
			}
			else
			{
				// 이동 애니메이션 재생
				if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
					animator.Play("Run");

				MoveTowards(target);
			}
		}
		else
		{
			// 대상 없을 때 이동 중지
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
				animator.Play("Idle");
		}

		if (stat.currentHP <= 0 && !isDead)
		{
			isDead = true;
			animator.Play("Die");
		}
	}

    IEnumerator Attack(GameObject enemy)
	{
		animator.Play("NormalAttack01");

		// 공격 타이밍에 맞춰 활성화
		attackPoint.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.2f); // 애니메이션 타이밍에 맞춰 조절

		// 여기에 충돌 판정 또는 데미지 처리 로직이 들어갈 수 있음
		stat.GainMP(20);

		// 짧은 시간 후 비활성화
		yield return new WaitForSeconds(0.1f);
		attackPoint.gameObject.SetActive(false);
	}

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

	void FaceTargetY(GameObject target)
	{
		Vector3 direction = target.transform.position - transform.position;
		direction.y = 0f; // 수직 방향 제거
		if (direction != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
		}
	}

    void MoveTowards(GameObject target)
	{
		FaceTargetY(target); // Y축만 회전
		transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * 5f);
	}
}

