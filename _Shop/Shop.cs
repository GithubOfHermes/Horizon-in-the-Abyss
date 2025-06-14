using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("UI")]
    public GameObject shopUI;                  // Shop_UI 패널
    public Button    cancelButton;             // 취소 버튼
    public Button    rerollButton;             // 리롤 버튼
    public Text      rerollButtonText;         // 리롤 버튼 텍스트

    [Header("상품 슬롯들")]
    public List<ShopProductSlot> productSlots; // 커스텀 슬롯 리스트 (Inspector에서 1~4 연결)

    [Header("리롤 설정")]
    public int    baseRerollCost  = 50;        // 기본 리롤 비용
    public int    maxRerollCost   = 400;       // 리롤 최대 비용
    private int   currentRerollCost;
    private bool  firstOpen       = false;     // 최초 오픈 플래그

    [Header("랜덤 할인율 (최초 오픈시)")]
    // 20%, 40%, 60% 할인 → 0.8f, 0.6f, 0.4f 곱하기
    private float[] discountRates = new float[] { 0.8f, 0.6f, 0.4f };
    private float   shopDiscount;             // 최초 오픈시 랜덤 선택된 할인율

    //==================================================================
    void Awake()
    {
        // 1) 초기에는 UI 비활성화
        shopUI.SetActive(false);

        // 2) 리롤 비용 초기화
        currentRerollCost = baseRerollCost;

        // 3) 버튼 리스너 연결
        cancelButton.onClick.AddListener(CloseShop);
        rerollButton.onClick .AddListener(OnReroll);

        // 4) 각 상품 슬롯의 구매 버튼 리스너 등록
        for (int i = 0; i < productSlots.Count; i++)
        {
            int idx = i; // 클로저 캡처 방지
            productSlots[i].buyButton.onClick.AddListener(() => OnBuy(idx));
        }

        UpdateRerollButton();
    }

    //==================================================================
    // “Shop” 태그 오브젝트를 클릭하면 실행되도록
    void OnMouseDown()
    {
        OpenShop();
    }

    //==================================================================
    // 1) 상점 열기
    public void OpenShop()
    {
        shopUI.SetActive(true);

        if (!firstOpen)
        {
            firstOpen     = true;
            shopDiscount  = discountRates[ Random.Range(0, discountRates.Length) ];
            GenerateAllProducts();        // 최초 상품 생성 + 할인
        }
    }

    //==================================================================
    // 2) 상점 닫기 (Cancel 버튼)
    public void CloseShop()
    {
        shopUI.SetActive(false);
    }

    //==================================================================
    // 3) 상품 전부 생성(랜덤 + 최초 할인)
    private void GenerateAllProducts()
    {
        // 첫 번째 슬롯만 최초 할인, 나머지는 정상가
		for (int i = 0; i < productSlots.Count; i++)
		{
			var slot = productSlots[i];

			// (1) 랜덤 장비 생성
			GadgetItem g = CreateRandomGadget();

			// (2) 기본 가격
			int basePrice = GetBasePrice(g);

			if (i == 0)
			{
				// 첫 슬롯: shopDiscount 적용
				float rawPrice     = basePrice * shopDiscount;            // 할인 적용된 실수 가격
				int   finalPrice   = Mathf.FloorToInt(rawPrice / 10f) * 10; // 10단위로 내림

				slot.Setup(g, finalPrice);

				// 할인 UI 활성화 및 텍스트 표시
				if (slot.saleImage != null) slot.saleImage.gameObject.SetActive(true);
				if (slot.saleText  != null)
					slot.saleText.text = Mathf.RoundToInt((1f - shopDiscount) * 100f) + "%";
			}
			else
			{
				// 나머지 슬롯: 정상가
				slot.Setup(g, basePrice);
				// 할인 UI 숨김 (ShopProductSlot.Setup에서 기본 숨김 처리)
			}
		}
    }

    //==================================================================
    // 4) 리롤 버튼 누를 때
    private void OnReroll()
    {
        // 4-1) 코인 검사
        var ps = FindObjectOfType<PlayerStat>();
        if (ps.currentCoin < currentRerollCost)
        {
            Debug.Log("코인이 부족합니다.");
            return;
        }

        // 4-2) 비용 차감 및 UI 갱신
        ps.currentCoin -= currentRerollCost;
        currentRerollCost *= 2;
        UpdateRerollButton();

		// 리롤할 때마다 첫 슬롯 할인율 랜덤 재선정
	    shopDiscount = discountRates[ Random.Range(0, discountRates.Length) ];
    
	    // 4-3) 모든 슬롯 재생성 & 비활성화된 슬롯도 다시 활성화
		for (int i = 0; i < productSlots.Count; i++)
		{
			var slot = productSlots[i];
			slot.gameObject.SetActive(true);

			GadgetItem g = CreateRandomGadget();
			int basePrice = GetBasePrice(g);

			if (i == 0)
			{
				float rawPrice     = basePrice * shopDiscount;
				int   finalPrice   = Mathf.FloorToInt(rawPrice / 10f) * 10;
				slot.Setup(g, finalPrice);
				if (slot.saleImage != null) slot.saleImage.gameObject.SetActive(true);
				if (slot.saleText  != null)
					slot.saleText.text = Mathf.RoundToInt((1f - shopDiscount) * 100f) + "%";
			}
			else
			{
				slot.Setup(g, basePrice);
			}
		}
	}

	private void UpdateRerollButton()
	{
		rerollButtonText.text = currentRerollCost.ToString();

		bool canReroll = (currentRerollCost <= maxRerollCost);
		
		rerollButton.transform.parent.gameObject.SetActive(canReroll);
    }

    //==================================================================
    // 5) 구매 버튼 누를 때
    private void OnBuy(int index)
    {
        var slot = productSlots[index];
        var ps   = FindObjectOfType<PlayerStat>();

        if (ps.currentCoin < slot.price)
        {
            Debug.Log("코인이 부족하여 구매 실패");
            return;
        }

        // 5-1) 코인 차감, 인벤토리에 아이템 추가, UI 업데이트
        ps.currentCoin -= slot.price;
        FindObjectOfType<Inventory>().AddItem(slot.gadget);
        slot.gameObject.SetActive(false);
    }

    //==================================================================
    // 6) 랜덤 장비 생성 함수 (등급 확률 + 아이콘/설정)
    public GadgetItem CreateRandomGadget()
	{
		int roll = Random.Range(0, 100);

		// ① 5% 확률로 GadgetRemove 생성
		if (roll < 5)
		{
			GadgetItem g = ScriptableObject.CreateInstance<GadgetItem>();
			g.gadgetType = GadgetType.GadgetRemove;   // GadgetRemove 타입
			g.gadgetRank = 1;                         // 등급 무시 (기본 1)
			g.itemName   = "Remover";            	  // 장비 이름름
			g.icon       = Resources.Load<Sprite>("GadgetRemove"); // Resources/GadgetRemove.png
			return g;
		}

		// ② 나머지 95% 중에서 등급 확률: 70% / 20% / 5%
		int r2 = Random.Range(0, 100);
		int rank = (r2 < 70 ? 1 : r2 < 90 ? 2 : 3);    // 0–69:1성, 70–89:2성, 90–94:3성

		// ③ 타입 선택 (sword, wand, bow, armor 중 랜덤)
		GadgetType[] types = new GadgetType[]
		{
			GadgetType.sword,
			GadgetType.wand,
			GadgetType.bow,
			GadgetType.armor
		};
		GadgetType type = types[Random.Range(0, types.Length)];

		// ④ 인스턴스 생성 및 설정
		GadgetItem g2 = ScriptableObject.CreateInstance<GadgetItem>();
		g2.gadgetType = type;
		g2.gadgetRank = rank;
		string key    = type.ToString().ToLower() + "_" + rank;  // 예: "sword_2"
		g2.itemName   = key;
		g2.icon       = Resources.Load<Sprite>(key);            // Resources/…/sword_2.png

		return g2;
	}

    //==================================================================
    // 7) 등급·타입별 기본 가격 반환
    private int GetBasePrice(GadgetItem g)
    {
        if (g.gadgetType == GadgetType.GadgetRemove)
            return 150;

        switch (g.gadgetRank)
        {
            case 1: return 100;
            case 2: return 200;
            case 3: return 500;
            default:return 0;
        }
    }
}