using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("UI 참조")]
    public Slider Boss_HP;        // ■ 수정: 체력 슬라이더 연결
    public Text   Boss_HP_Text;   // ■ 수정: 체력 텍스트 연결
    public Slider Boss_MP1;       // ■ 수정: 스킬1 MP 슬라이더 연결
    public Slider Boss_MP2;       // ■ 수정: 스킬2 MP 슬라이더 연결

    [Header("MP 설정")]
    public int maxMP1 = 6;        // ■ 수정: 스킬1 쿨다운(6초)
    public int maxMP2 = 12;       // ■ 수정: 스킬2 쿨다운(12초)
    private int currMP1 = 0;
    private int currMP2 = 0;
    private float mpTimer1 = 0f;
    private float mpTimer2 = 0f;

    [Header("패턴 설정")]
    public float attackInterval = 2f;   // ■ 수정: 기본 공격 주기
    private float attackTimer = 0f;

    private Animator animator;          // ■ 수정: Animator 컴포넌트 참조
    private EnemyStat enemyStat;        // ■ 수정: HP/데미지 정보
    private GridManager gridManager;    // ■ 수정: GridManager 참조
    private bool isDead = false;
    private bool isFightStarted = false;

    [Header("스킬1 참조")]
    public GameObject skill1_1;         // ■ 수정: Boss_Skill1_1 오브젝트
    public GameObject skill1_2;         // ■ 수정: Boss_Skill1_2 오브젝트

    [Header("스턴 설정")]
    public float stunDuration = 2f;     // ■ 수정: 스킬2 스턴 지속 시간

    [Header("보상 및 클리어")]
    public GameObject playerPrefab;     // ■ 수정: 기존 Player_Prefab
    public GameObject player2Prefab;    // ■ 수정: Player2_Prefab
    public GameObject demoClearCanvas;  // ■ 수정: DemoClear_Canvas 참조

	private bool isHit = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyStat = GetComponent<EnemyStat>();
        
        // ========== 수정: Player_Floor 오브젝트에서 GridManager 가져오기 ============
        GameObject playerFloorObj = GameObject.Find("Player_Floor");
        if (playerFloorObj != null)
            gridManager = playerFloorObj.GetComponent<GridManager>();
        else
            gridManager = FindObjectOfType<GridManager>();

        // ========== 수정: Inspector에 할당되지 않은 demoClearCanvas를 Tag="Demo"인 Canvas로 자동 할당 ============
        if (demoClearCanvas == null)
            demoClearCanvas = GameObject.FindGameObjectWithTag("Demo");

        // ========== 수정: Tag="Demo"인 Canvas를 부모로 설정 ============
        GameObject demoRoot = GameObject.FindGameObjectWithTag("Demo");
        if (demoRoot != null && demoClearCanvas != null)
            demoClearCanvas.transform.SetParent(demoRoot.transform, false);
    }

    void Start()
    {
        // ■ HP/MP 바 초기값 설정
        Boss_HP.maxValue = enemyStat.Enemy_MaxHP;
        Boss_HP.value    = enemyStat.Enemy_currentHP;
        Boss_HP_Text.text = enemyStat.Enemy_currentHP + "/" + enemyStat.Enemy_MaxHP;

        Boss_MP1.maxValue = maxMP1;
        Boss_MP1.value    = currMP1;
        Boss_MP2.maxValue = maxMP2;
        Boss_MP2.value    = currMP2;
    }

    void Update()
    {
        if (!isFightStarted || isDead) return;

        // ■ HP UI 업데이트
        Boss_HP.value    = Mathf.Clamp(enemyStat.Enemy_currentHP, 0, enemyStat.Enemy_MaxHP);
        Boss_HP_Text.text = enemyStat.Enemy_currentHP + "/" + enemyStat.Enemy_MaxHP;

        // ■ MP 회복
        mpTimer1 += Time.deltaTime;
        if (mpTimer1 >= 1f) { mpTimer1 -= 1f; currMP1 = Mathf.Min(currMP1+1, maxMP1); Boss_MP1.value = currMP1; }
        mpTimer2 += Time.deltaTime;
        if (mpTimer2 >= 1f) { mpTimer2 -= 1f; currMP2 = Mathf.Min(currMP2+1, maxMP2); Boss_MP2.value = currMP2; }

        // ■ 스킬1 사용 (6초마다)
        if (currMP1 >= maxMP1)
        {
            UseSkill1();
            currMP1 = 0; Boss_MP1.value = 0; mpTimer1 = 0f;
            attackTimer = 0f; return;   // ■ 스킬 후 Update 종료
        }
        // ■ 스킬2 사용 (12초마다)
        if (currMP2 >= maxMP2)
        {
            UseSkill2();
            currMP2 = 0; Boss_MP2.value = 0; mpTimer2 = 0f;
            attackTimer = 0f; return;
        }

        // ■ 기본 공격 (2초마다)
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer -= attackInterval;
            Attack();
        }
    }

    public void StartFight()
    {
        isFightStarted = true;      // ■ 전투 시작 플래그
        animator.Play("Boss_Idle");
    }

    void Attack()
    {
        animator.Play("Boss_Attack");  // ■ 일반 공격 애니메이션
        GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			var ps = player.GetComponent<PlayerStat>();
			if (ps != null)
				ps.TakeDamage(enemyStat.Enemy_attack);  // Boss의 공격력만큼 데미지 호출
		}
    }

    void UseSkill1()
    {
        animator.Play("Boss_Skill1");  // ■ 스킬1 애니메이션
        StartCoroutine(ActivateSkillObjects());
    }

	IEnumerator ActivateSkillObjects()
	{
		skill1_1.SetActive(true);
		yield return new WaitForSeconds(0.3f);
		skill1_1.SetActive(false);
		skill1_2.SetActive(true);
		yield return new WaitForSeconds(0.3f);
		skill1_2.SetActive(false);
		animator.Play("Boss_Idle"); 
    }

    void UseSkill2()
    {
        animator.Play("Boss_Skill2");  // ■ 스킬2 애니메이션
        StartCoroutine(StunPlayers());
    }

	IEnumerator StunPlayers()
	{
		var players = GameObject.FindGameObjectsWithTag("Player");
		foreach (var p in players)
		{
			var pc = p.GetComponent<PlayerController>();

			if (pc != null)
			{
				pc.animator.Play("Idle");
				pc.enabled = false; // ■ 스턴: 컨트롤 비활성화
			}
		}
		yield return new WaitForSeconds(stunDuration);
		foreach (var p in players)
		{
			var pc = p.GetComponent<PlayerController>();
			if (pc != null) pc.enabled = true;  // ■ 스턴 해제
		}
		animator.Play("Boss_Idle");  
    }

	public void TakeDamage(int damage)
	{
		if (isHit) return;
		isHit = true;

		enemyStat.Enemy_currentHP -= damage;
		if (enemyStat.Enemy_currentHP <= 0)
			Die();
		StartCoroutine(ResetHitFlag()); 
    }

	private IEnumerator ResetHitFlag()
    {
        yield return new WaitForSeconds(0.1f);
        isHit = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        animator.Play("Boss_Die");       // ■ 사망 애니메이션
        StartCoroutine(HandleBossDefeat());
    }

    IEnumerator HandleBossDefeat()
    {
        // ========== 수정: Boss 사망 시 Clear_BGM으로 전환 ============
        SoundManager.Instance.PlayBGM(SoundManager.BGMType.Clear_BGM);

        // ■ 플레이어 HP 전부 회복
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            var ps = p.GetComponent<PlayerStat>();
            if (ps != null) ps.currentHP = ps.maxHP;
        }
        // ■ Player2_Prefab 생성 (격자 위 빈 위치)
        Vector3 spawnPos = gridManager.GetEmptyPosition(playerPrefab.transform.position);
        Instantiate(player2Prefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        // ■ DemoClear_Canvas FadeIn
        demoClearCanvas.SetActive(true);
        var fe = demoClearCanvas.GetComponent<FadeEffect>();
        if (fe != null) fe.FadeIn();
    }
}

