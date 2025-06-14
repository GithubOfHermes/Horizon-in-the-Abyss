using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// ■ 새로 생성: Elite 전용 동작 제어 스크립트
public class EliteManager : MonoBehaviour
{
    [Header("UI 참조")]
    public Slider Elite_HP;            // ■ 수정: 슬라이더 컴포넌트 연결
    public Text   Elite_HP_Text;       // ■ 수정: 체력 텍스트 연결
    public Slider Elite_MP;            // ■ 수정: MP 슬라이더 연결

    [Header("MP 설정")]
    public int maxMP = 12;             // ■ 수정: 최대 MP 설정 (12초마다 스킬)
    private int   currentMP = 0;
    private float mpTimer   = 0f;

    private EnemyStat enemyStat;       // ■ 수정: EnemyStat 참조 :contentReference[oaicite:0]{index=0}
    private Animation animationComp;   // ■ 수정: Animation Component 참조
    private GridManager gridManager;   // ■ 수정: GridManager 참조 :contentReference[oaicite:1]{index=1}

    private float attackTimer = 0f;
    private bool  isDead      = false;
    private bool  isFightStarted = false;

    private bool isHit = false; // ■ 수정: 연속 데미지 방지 플래그 추가

    void Awake()
    {
        // EnemyStat 컴포넌트 가져오기
        enemyStat = GetComponent<EnemyStat>(); 
    }

    void Start()
    {
        // Animation 및 GridManager 참조
        animationComp = GetComponent<Animation>();
        gridManager   = FindObjectOfType<GridManager>();

        // HP/MP 바 초기값 설정
        if (Elite_HP != null)
        {
            Elite_HP.maxValue = enemyStat.Enemy_MaxHP;     // ■ HP 최대치 설정
            Elite_HP.value    = enemyStat.Enemy_currentHP; // ■ 현재 HP 설정
        }
        if (Elite_HP_Text != null)
            Elite_HP_Text.text = enemyStat.Enemy_currentHP + "/" + enemyStat.Enemy_MaxHP;

        if (Elite_MP != null)
        {
            Elite_MP.maxValue = maxMP;   // ■ MP 최대치 설정
            Elite_MP.value    = currentMP;
        }
    }

    void Update()
    {
        if (!isFightStarted || isDead) return;

        // --- HP 바 업데이트 ---
        if (Elite_HP != null)
            Elite_HP.value = Mathf.Clamp(enemyStat.Enemy_currentHP, 0, enemyStat.Enemy_MaxHP);
        if (Elite_HP_Text != null)
            Elite_HP_Text.text = enemyStat.Enemy_currentHP + "/" + enemyStat.Enemy_MaxHP;

        // --- MP 회복 및 스킬 사용 ---
        mpTimer += Time.deltaTime;
        if (mpTimer >= 1f)
        {
            mpTimer -= 1f;
            currentMP = Mathf.Min(currentMP + 1, maxMP);
            if (Elite_MP != null)
                Elite_MP.value = currentMP;
        }
		
		// MP가 최대치에 도달했을 때 스킬 사용 후 즉시 Update를 종료
        if (currentMP >= maxMP)
		{
			UseSkill();                           // 스킬 애니메이션 재생 (Elite_Attack1)
			currentMP = 0;                        // MP 리셋
			if (Elite_MP != null)
				Elite_MP.value = 0;
			mpTimer = 0f;                         // MP 회복 타이머 리셋
			attackTimer = 0f;                     // ■ 수정: 공격 타이머 초기화
			return;                               // ■ 수정: 일반 공격(Attack) 로직 실행 방지
		}

        // --- 일반 공격 (2초 주기) ---
        attackTimer += Time.deltaTime;
        if (attackTimer >= 2f)
        {
            attackTimer -= 2f;
            Attack();
        }
    }

    // ■ 전투 시작 신호 함수 (FightButton에서 호출)
    public void StartFight()
    {
        isFightStarted = true;
    }

    // 일반 공격 구현
    private void Attack()
    {
        if (animationComp != null)
            animationComp.Play("Elite_Attack2"); // ■ 일반 공격 애니메이션
        // 플레이어에게 데미지 적용
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var ps = player.GetComponent<PlayerStat>();
            if (ps != null)
                ps.TakeDamage(enemyStat.Enemy_attack);
        }
    }

    // 스킬 사용 구현
    private void UseSkill()
    {
        if (animationComp != null)
            animationComp.Play("Elite_Attack1"); // ■ 스킬 애니메이션

        // 그리드 기준으로 플레이어 밀치기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && gridManager != null)
        {
            Vector3 origin   = gridManager.originPosition;
            float   size     = gridManager.cellSize;
            // 현재 그리드 좌표
            Vector3 localPos = player.transform.position - origin;
            int gridX = Mathf.RoundToInt(localPos.x / size);
            int gridZ = Mathf.RoundToInt(localPos.z / size);
            // 최대 X: +3, Z: +0 적용
            int targetX = Mathf.Min(gridX + 3, gridManager.maxX);
            int targetZ = Mathf.Clamp(gridZ, gridManager.minZ, gridManager.maxZ);
            Vector3 endPos = origin + new Vector3(targetX * size, 0f, targetZ * size);
            StartCoroutine(FlyPlayer(player, player.transform.position, endPos));
        }
    }

    // 플레이어를 포물선 경로로 날려보내는 코루틴
    private IEnumerator FlyPlayer(GameObject player, Vector3 start, Vector3 end)
    {
        float duration = 0.5f;
        float elapsed  = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float height = Mathf.Sin(t * Mathf.PI) * 2f;
            player.transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;
            yield return null;
        }
    }

    // 데미지 처리
    public void TakeDamage(int damage)
    {
        if (isHit) 
            return;           // ■ 수정: 이미 처리된 상태면 무시

        isHit = true;         // ■ 수정: 첫 데미지 처리 시 플래그 설정
        Debug.Log("EliteManager.TakeDamage 호출, damage: " + damage); 

        enemyStat.Enemy_currentHP -= damage;
        if (enemyStat.Enemy_currentHP <= 0)
            Die();

        StartCoroutine(ResetHitFlag()); 
    }

    // ■ 수정: 짧은 지연 후 데미지 플래그를 해제하는 코루틴 추가
    private IEnumerator ResetHitFlag()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 후
        isHit = false;                         // 플래그 해제
    }

    // 사망 처리
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        if (animationComp != null)
            animationComp.Play("Elite_Die"); // ■ 사망 애니메이션
        Destroy(gameObject, 1f);
    }
}
