using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightButton : MonoBehaviour
{
    public Button fightButton;

    private bool hadEnemies;
    private bool isEliteRoom; // Elite 방 판정용 플래그 추가

    void Start()
    {
        fightButton.onClick.AddListener(OnFightStart);
    }

    void OnFightStart()
    {
        fightButton.gameObject.SetActive(false);
        var players = GameObject.FindGameObjectsWithTag("Player");
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");    // Enemy 태그
        var elites = GameObject.FindGameObjectsWithTag("Elite");    // Elite 태그
        var bosses = GameObject.FindGameObjectsWithTag("Boss");     // Boss 태그

        hadEnemies = enemies.Length > 0;
        isEliteRoom = elites.Length > 0;

        if (enemies.Length == 0 && elites.Length == 0 && bosses.Length == 0)
        {
            ResetPlayers(players);
            return;
        }

        // 전투 시작 처리
        foreach (var p in players)
        {
            var drag = p.GetComponent<PlayerDraggable>();
            if (drag != null)
            {
                drag.isDraggable = false;  // 수정: 드래그 비활성화
                drag.enabled = false;
            }
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wall"), false);
            var ctrl = p.GetComponent<PlayerController>();
            if (ctrl != null)
                ctrl.enabled = true;  // 수정: 플레이어 컨트롤 활성화
        }

        // ■ Enemy 전투 시작 처리
        foreach (var enemy in enemies)
        {
            var em = enemy.GetComponent<EnemyManager>();
            if (em != null)
                em.StartFight();  // 수정: 적 AI 시작
        }

        // ■ Elite 전투 시작 처리
        foreach (var elite in elites)
        {
            var em = elite.GetComponent<EliteManager>(); // EliteManager 스크립트 참조
            if (em != null)
                em.StartFight(); // 수정: Elite 전투 시작
        }

        // ■ 수정: Boss 전투 시작 처리 추가 (Enemy, Elite와 동일한 위치)
        foreach (var b in bosses)
        {
            var bm = b.GetComponent<BossManager>();
            if (bm != null) bm.StartFight();
        }

        StartCoroutine(MonitorEndFight(players));  // 수정: 전투 종료 모니터링 코루틴 호출
    }

    IEnumerator MonitorEndFight(GameObject[] players)
    {
        if (players == null)
        {
            Debug.LogError("MonitorEndFight: players 배열이 null입니다!");
            yield break;
        }
        Debug.Log("MonitorEndFight: players.Length = " + players.Length);

        // 수정: Enemy, Elite, Boss 태그가 모두 사라질 때까지 대기
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0
            || GameObject.FindGameObjectsWithTag("Elite").Length > 0
            || GameObject.FindGameObjectsWithTag("Boss").Length > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        ResetPlayers(players);
        
        // ========== 수정: 전투 종료 후 Game_BGM으로 전환 ============
        SoundManager.Instance.PlayBGM(SoundManager.BGMType.Game_BGM);


        // ■ 수정: 일반 방(Enemy만 있었던 방) 클리어 시 Enemy_Spawner 오브젝트 비활성화
        if (hadEnemies && !isEliteRoom)
        {
            GameObject spawner = GameObject.Find("Enemy_Spawner");  // 씬에 있는 Enemy_Spawner 오브젝트 이름
            if (spawner != null)
                spawner.SetActive(false);  // 스포너 비활성화
        }

        // ==================== 수정: RewardManager.Instance null 체크 ====================
        if (RewardManager.Instance != null)
        {
            RewardManager.Instance.HandleRoomClear(hadEnemies, isEliteRoom);
        }
        else
        {
            Debug.LogError("RewardManager.Instance가 null입니다. 씬에 RewardManager 오브젝트가 있는지 확인하세요.");
        }

        // 전투 종료 시 미니맵 토글 버튼 및 아이콘 재활성화
        if (hadEnemies && !isEliteRoom)
        {
            StageManager.Instance.ApplyClearState();
        }

        if (!hadEnemies && isEliteRoom)
        {
            StageManager.Instance.ApplyClearState();
        }
    }

    // 플레이어 상태 초기화 로직 분리
    void ResetPlayers(GameObject[] players)
    {
        foreach (var p in players)
        {
            var drag = p.GetComponent<PlayerDraggable>();
            if (drag != null)
            {
                p.transform.position = drag.lastPosition;  // 마지막 드롭 위치로 복귀
                drag.isDraggable = true;  // 드래그 재활성화
                drag.enabled = true;
            }
            var anim = p.GetComponent<Animator>();
            if (anim != null)
                anim.Play("Idle");  // Idle 애니메이션 재생

            p.transform.rotation = Quaternion.Euler(0f, 0f, 0f);  // 수정: Rotation 초기화

            var pc = p.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = false;  // 전투 모드 해제 시 PlayerController 비활성화
        }
    }
}