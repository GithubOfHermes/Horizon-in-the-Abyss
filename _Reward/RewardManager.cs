using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    [Header("보상 UI")]
    public GameObject rewardUI;         // Reward_UI 오브젝트
    public Image[]   rewardImages;      // Reward_1_Img, Reward_2_Img, Reward_3_Img
    public Text[]    rewardTexts;       // Reward_1_Text,  Reward_2_Text,  Reward_3_Text
    public Button[]  rewardButtons;     // Reward_1_Button, Reward_2_Button, Reward_3_Button

    // ==================== 일반 방 클리어 카운터 추가 ====================
    private int normalClearCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        rewardUI.SetActive(false);
    }

	/// <summary>
	/// 방 클리어가 발생했을 때 호출
	/// </summary>
	/// <param name="isEliteRoom">중간보스 방인지 여부</param>
	public void HandleRoomClear(bool hadEnemies, bool isEliteRoom)
	{
		// ==================== 플레이어 체력 회복 적용 ====================
		var ps = FindObjectOfType<PlayerStat>();
		if (ps != null)
		{
			int healPercent = isEliteRoom ? 40 : 15;  // 40% or 15%
			int amount = Mathf.FloorToInt(ps.maxHP * healPercent / 100f);
			ps.currentHP = Mathf.Min(ps.currentHP + amount, ps.maxHP);
		}

		if (isEliteRoom)
		{
			// ==================== 엘리트 방 보상 즉시 표시 ====================
			ShowRareRewards();
		}
		else if (hadEnemies)  // ==================== 적이 있었던 방만 '일반 방'으로 처리 ====================
		{
			normalClearCount++;
			Debug.Log("일반 방 ClearCount: " + normalClearCount);
			if (normalClearCount >= 3)
			{
				normalClearCount = 0;
				ShowNormalRewards();
			}
		}
		// else: 적도 없고 엘리트도 아니면 보상 없음
    }

    // 일반 보상: 상점 확률과 동일하게 생성
    private void ShowNormalRewards()
    {
        // ========== 수정: FindObjectOfType 대신 StageManager.Instance.shop 참조, null 체크 추가 ============
        Shop shop = (StageManager.Instance != null ? StageManager.Instance.shop : null);
        if (shop == null)
        {
            Debug.LogError("ShowNormalRewards: Shop 인스턴스를 찾을 수 없습니다. StageManager.shop이 할당되었는지 확인하세요.");
            return;
        }
        // ========== 수정: 보상 아이템 저장용 리스트 선언 ============
        List<GadgetItem> items = new List<GadgetItem>();
        for (int i = 0; i < 3; i++)
        {
            // ========== 수정: Shop.CreateRandomGadget으로 아이템 추가 ============
            items.Add(shop.CreateRandomGadget());
        }
        // ========== 수정: SetupRewardUI에 items 전달 ============
        SetupRewardUI(items);
    }

    // 레어 보상: 중간보스 전용 확률로 3개 생성
    private void ShowRareRewards()
    {
        List<GadgetItem> items = new List<GadgetItem>();
        for (int i = 0; i < 3; i++)
        {
            items.Add(CreateRareGadget());  // 수정: 레어 확률 적용
        }
        SetupRewardUI(items);
    }

    // 중간보스 전용 확률 (1성: 45%, 2성: 35%, 3성: 15%, Remove: 5%)
    private GadgetItem CreateRareGadget()
    {
        int roll = Random.Range(0, 100);
        if (roll < 5)
            return CreateGadget(GadgetType.GadgetRemove, 1);

        roll = Random.Range(0, 100);
        int rank = (roll < 45 ? 1 : roll < 80 ? 2 : 3);
        // 타입 랜덤
        var types = new [] { GadgetType.sword, GadgetType.wand, GadgetType.bow, GadgetType.armor };
        var type  = types[Random.Range(0, types.Length)];
        return CreateGadget(type, rank);
    }

    // 공통 생성 헬퍼
    private GadgetItem CreateGadget(GadgetType type, int rank)
    {
        var g = ScriptableObject.CreateInstance<GadgetItem>();
        g.gadgetType = type;
        g.gadgetRank = rank;
        string key = type.ToString().ToLower() + "_" + rank;
        g.itemName       = key;
        g.icon           = Resources.Load<Sprite>(key);
        return g;
    }

    // 보상 UI 세팅
    private void SetupRewardUI(List<GadgetItem> items)
	{
		rewardUI.SetActive(true);                      // UI 보이기
		for (int i = 0; i < 3; i++)
		{
			rewardImages[i].sprite = items[i].icon;    // 이미지
			rewardTexts[i].text    = items[i].itemName;// 이름

			int idx = i;
			rewardButtons[i].onClick.RemoveAllListeners();
			rewardButtons[i].onClick.AddListener(() =>
			{
				FindObjectOfType<Inventory>().AddItem(items[idx]); // 인벤토리 추가
				rewardUI.SetActive(false);                        // UI 닫기
			});
		}
	}
}
