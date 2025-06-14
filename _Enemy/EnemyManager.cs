using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum Attack_Type { Close, Far }
    public Attack_Type attackType;

    private EnemyStat enemyStat;
    private Animator animator;
    private Animation legacyAnim;
    private GameObject player;
    private float attackTimer;
    private bool isDead = false;
    private bool isFightStarted = false;
    private bool isAttacking = false;

    public Transform Enemy_attackPoint;
    public GameObject Enemy_FireBall;

    private bool isHit = false; // 피해 중복 방지를 위한 플래그

    void Start()
    {
        enemyStat = GetComponent<EnemyStat>();
        enemyStat.Enemy_currentHP = enemyStat.Enemy_MaxHP;
        animator = GetComponent<Animator>();
        legacyAnim = GetComponent<Animation>();
        player = GameObject.FindGameObjectWithTag("Player");
        attackTimer = enemyStat.Enemy_attackSpeed;
    }

    void Update()
    {
        if (isDead || !isFightStarted || player == null || isAttacking)
            return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > enemyStat.Enemy_attackRange)
        {
            LookAtPlayer(); PlayRun(); MoveToPlayer();
        }
        else
        {
            LookAtPlayer();
            attackTimer += Time.deltaTime;
            if (attackTimer >= enemyStat.Enemy_attackSpeed)
            {
                attackTimer = 0f;
                StartCoroutine(PerformAttack());
            }
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (animator != null) animator.Play("Enemy_Attack");
        else if (legacyAnim != null) legacyAnim.Play("Enemy_Attack");

        if (attackType == Attack_Type.Close && Enemy_attackPoint != null)
            Enemy_attackPoint.gameObject.SetActive(true);

        if (attackType == Attack_Type.Far && Enemy_FireBall != null)
        {
            GameObject fb = Instantiate(Enemy_FireBall, Enemy_attackPoint.position, transform.rotation);
            var fire = fb.GetComponent<Fireball>();               // 수정: Fireball 스크립트 참조
            if (fire != null)
            {
                fire.SetTarget(player.transform);                // 수정: 대상 전달
                fire.SetDamage(enemyStat.Enemy_attack);          // 수정: 데미지 전달
            }
            Destroy(fb, 2f);
        }

        yield return new WaitForSeconds(0.8f);
        if (attackType == Attack_Type.Close && Enemy_attackPoint != null)
            Enemy_attackPoint.gameObject.SetActive(false);

        isAttacking = false;
    }

    void DropRewardAndDestroy()
    {
        var pStat = player.GetComponent<PlayerStat>();
        var pLevel = player.GetComponent<PlayerLevel>();
        if (pStat != null) pStat.currentCoin += enemyStat.Enemy_Coin;
        if (pLevel != null) pLevel.GainExp(enemyStat.Enemy_Exp);

        Destroy(gameObject);
    }

    void LookAtPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        dir.y = 0f;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 3f);
    }

    void PlayRun()
    {
        if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Run"))
            animator.Play("Enemy_Run");
        else if (legacyAnim != null)
            legacyAnim.Play("Enemy_Run");
    }

    public void Enemy_TakeDamage(int damage)
    {
        if (isDead || isHit) return;
        isHit = true;
        enemyStat.Enemy_currentHP -= damage;
        if (enemyStat.Enemy_currentHP <= 0)
            Enemy_Die();
        StartCoroutine(ResetHitFlag());
    }

    private IEnumerator ResetHitFlag()
    {
        yield return new WaitForSeconds(0.1f);
        isHit = false;
    }

    public void Enemy_Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();                                        // 수정: 진행 중 공격 코루틴 정지
        if (animator != null) animator.Play("Enemy_Die");
        else if (legacyAnim != null) legacyAnim.Play("Enemy_Die");
        Invoke("DropRewardAndDestroy", 1.0f);
    }

    public void StartFight()
    {
        isFightStarted = true;                                       // 수정: 전투 시작 플래그 활성화
    }
}
