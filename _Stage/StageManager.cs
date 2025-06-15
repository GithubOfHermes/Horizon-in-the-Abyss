using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 전반의 방 전환 및 상태 관리를 담당하는 매니저
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("공용 레퍼런스")]
    public FightButton fightButton;
    public EnemySpawner enemySpawner;
    public Shop shop;
    public EliteManager elitePrefab;
    public Transform eliteContainer;
    public BossManager bossPrefab;
    public Transform bossContainer;

    [Header("진행도 카운트")]               // 수정: 클리어 카운트 변수 추가
    public int clearCount = 0;             // Normal 클리어 횟수
    public int eliteClearCount = 0;        // Elite 클리어 횟수

    private RoomTrigger currentRoom;
    private GameObject currentElite;
    private GameObject currentBoss;

    void Awake()
    { 
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    // 시작 방 세팅 메서드
    void SetupStartRoom()
    {
        enemySpawner.gameObject.SetActive(false);
        fightButton.fightButton.gameObject.SetActive(false);
    }

    void SetupNormalRoom()
    {
        enemySpawner.gameObject.SetActive(true);
        fightButton.fightButton.gameObject.SetActive(true);

        // ========== 수정: Normal 방 입장 시 모든 EnemySpPoint의 SpawnEnemiesUnique() 호출 ============
        EnemySpPoint[] spPoints = FindObjectsOfType<EnemySpPoint>();
        foreach (var sp in spPoints)
        {
            sp.SpawnEnemiesUnique();
        }
    }


    // 플레이어가 방에 입장할 때 호출
    public void EnterRoom(RoomTrigger room)
    {
        // 이전 Shop 방 나갈 때 Shop_Prefab 및 ShopWindow 비활성화 
        if (currentRoom != null && currentRoom.roomType == RoomType.Shop)
        {
            shop.gameObject.SetActive(false);
            shop.shopUI.SetActive(false);
        }

        currentRoom = room;

        // Enemy/Elite/Boss 방 입장 시 UI·미니맵 비활성화
        if (room.roomType == RoomType.Normal
         || room.roomType == RoomType.Elite
         || room.roomType == RoomType.Boss)
        {
            var invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null) invUI.inventoryWindow.SetActive(false); // 수정: 인벤토리 UI 비활성화 :contentReference[oaicite:0]{index=0}

            MinimapManager.Instance.minimapToggleButton.gameObject.SetActive(false); // 수정: 미니맵 버튼 비활성화 :contentReference[oaicite:1]{index=1}
            MinimapManager.Instance.minimapPanel.SetActive(false);                 // 수정: 미니맵 패널 비활성화 :contentReference[oaicite:2]{index=2}
        }

        // 이미 클리어된 일반 방이면 클리어 상태 유지
        if (room.roomType == RoomType.Normal && room.isCleared)
        {
            ApplyClearState();
            return;
        }

        switch (room.roomType)
        {
            case RoomType.Start: SetupStartRoom(); break;
            case RoomType.Normal: SetupNormalRoom(); break;
            case RoomType.Shop: SetupShopRoom(); break;   // 수정: 상점 방
            case RoomType.Elite: SetupEliteRoom(); break;   // 수정: 엘리트 방
            case RoomType.Boss: SetupBossRoom(); break;   // 수정: 보스 방
        }

        // ========== 수정: Normal/Elite 방 입장 시 Fight_BGM으로 전환 ============
        if (room.roomType == RoomType.Normal || room.roomType == RoomType.Elite)
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.Fight_BGM);

        // ========== 수정: Boss 방 입장 시 Boss_BGM으로 전환 ============
        if (room.roomType == RoomType.Boss)
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.Boss_BGM);

        // ========== 수정: Shop 방 입장 시 Shop_BGM으로 전환 ============
        if (room.roomType == RoomType.Shop)
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.Shop_BGM);
    }

    // 상점 방 세팅
    void SetupShopRoom()
    {
        enemySpawner.gameObject.SetActive(false);                  // 수정: 일반 스포너 비활성화 :contentReference[oaicite:3]{index=3}
        fightButton.fightButton.gameObject.SetActive(false);       // 수정: 전투 버튼 비활성화 :contentReference[oaicite:4]{index=4}
        shop.gameObject.SetActive(true);                           // 수정: Shop_Prefab 활성화 :contentReference[oaicite:5]{index=5}
        shop.shopUI.SetActive(false);                              // 수정: Shop_UI 비활성화 :contentReference[oaicite:6]{index=6}
    }

    // 엘리트 방 세팅
    void SetupEliteRoom()
    {
        enemySpawner.gameObject.SetActive(false);                  // 수정: 일반 스포너 비활성화 :contentReference[oaicite:7]{index=7}
        if (currentElite == null)
            currentElite = Instantiate(elitePrefab.gameObject, eliteContainer); // 수정: 엘리트 프리팹 생성 :contentReference[oaicite:8]{index=8}
        currentElite.SetActive(true);                               // 수정: 엘리트 활성화 :contentReference[oaicite:9]{index=9}
        fightButton.fightButton.gameObject.SetActive(true);         // 수정: 전투 버튼 활성화 :contentReference[oaicite:10]{index=10}
    }

    // 보스 방 세팅
    void SetupBossRoom()
    {
        enemySpawner.gameObject.SetActive(false);                  // 수정: 일반 스포너 비활성화 :contentReference[oaicite:11]{index=11}
        if (currentBoss == null)
            currentBoss = Instantiate(bossPrefab.gameObject, bossContainer);    // 수정: 보스 프리팹 생성 :contentReference[oaicite:12]{index=12}
        currentBoss.SetActive(true);                                // 수정: 보스 활성화 :contentReference[oaicite:13]{index=13}
        fightButton.fightButton.gameObject.SetActive(true);         // 수정: 전투 버튼 활성화 :contentReference[oaicite:14]{index=14}
    }

    // 방 클리어 시 호출: 타입 변경·카운트·아이콘·UI 복구
    public void ApplyClearState()
    {
        if (currentRoom == null) return;

        var prev = currentRoom.roomType;
        currentRoom.isCleared = true;
        currentRoom.roomType = RoomType.Clear;

        // 전투 오브젝트 비활성화
        enemySpawner.gameObject.SetActive(false);
        fightButton.fightButton.gameObject.SetActive(false);

        // 카운트 증가
        if (prev == RoomType.Normal) clearCount++;
        if (prev == RoomType.Elite) eliteClearCount++;

        // 수정: currentRoom을 넘겨서 아이콘만 교체
        MinimapManager.Instance.UpdateIconState(currentRoom, RoomType.Clear);

        // 수정: 클리어 후 인벤토리 UI·미니맵 버튼 재활성화 
        MinimapManager.Instance.minimapToggleButton.gameObject.SetActive(true);

        // ========== 수정: 전투 후 미니맵 버튼 인터랙션 상태 재계산 ============
        MinimapManager.Instance.SetupMapAgain();
    }

    // … 나머지 메서드 (SetupStartRoom, SetupNormalRoom, SetupShopRoom, 등)
}

