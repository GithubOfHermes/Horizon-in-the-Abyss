using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 사용 인터페이스
public interface IUsableItem
{
    void Use();  // 아이템 사용 시 실행할 로직
}

// Inventory 스크립트: 아이템 보관 및 사용 관리
public class Inventory : MonoBehaviour
{
	[Header("인벤토리 설정")]
	[Tooltip("인벤토리 최대 슬롯 수")]
	public int capacity = 8;                              // 인벤토리 최대 슬롯
	[Tooltip("현재 보유 중인 아이템 리스트")]
	public List<Item> items = new List<Item>();           // 아이템 저장 리스트

	[Header("UI 연결")]
	[Tooltip("인벤토리 UI 스크립트 참조")]
	public InventoryUI inventoryUI;                       // 인벤토리 UI 스크립트 참조

	// 슬롯이 없을 때 가젯 아이템을 임시 보관할 큐
    private Queue<Item> pendingItems = new Queue<Item>();

	void Start()
	{
		// UI 초기화 및 슬롯 업데이트
		if (inventoryUI != null)
		{
			inventoryUI.Initialize(this);
			UpdateUI();
		}
		else
		{
			Debug.LogWarning("InventoryUI가 연결되지 않았습니다.");
		}
	}

	// 인벤토리에 아이템 추가
    public bool AddItem(Item item)
    {
        Item itemToAdd = item;
        GadgetItem g = item as GadgetItem;
        if (g != null)
        {
            // ScriptableObject 인스턴스 복제
            itemToAdd = Instantiate(g);  
        }

		// 용량 체크
        if (items.Count >= capacity)
        {
            // 수정: 패턴 매칭 대신 as 연산자 사용
            GadgetItem gem = itemToAdd as GadgetItem;
            if (gem != null)
            {
                pendingItems.Enqueue(itemToAdd);
                // 수정: 문자열 보간 대신 문자열 연결 사용
                Debug.Log("인벤토리가 가득 찼습니다. [" + gem.gadgetType + " 등급 " + gem.gadgetRank + "] 아이템을 대기열에 추가합니다.");
            }
            else
            {
                Debug.Log("인벤토리에 빈 슬롯이 없습니다.");
            }
            return false;
        }

        items.Add(itemToAdd);
        UpdateUI();
        return true;
    }

	// 대기 아이템 처리
	public void ProcessPendingItems()
	{
		// while문으로 대기열에 있는 모든 아이템을 FIFO 순서대로 빈 슬롯에 추가
        while (pendingItems.Count > 0 && items.Count < capacity)
        {
            Item next = pendingItems.Dequeue();
            items.Add(next);
            GadgetItem gem = next as GadgetItem;
            if (gem != null)
            {
                Debug.Log("대기 중이던 [" + gem.gadgetType + " 등급 " + gem.gadgetRank + "] 아이템을 인벤토리에 추가합니다.");
            }
        }
        UpdateUI();
	}

	// 인벤토리에서 아이템 제거
	public void RemoveItem(int index)
	{
		if (index < 0 || index >= items.Count) return;
		items.RemoveAt(index);
		ProcessPendingItems(); // 아이템 제거 후 대기 아이템 처리
		UpdateUI();  // UI 갱신
	}

	// 슬롯 클릭 시 아이템 사용 요청
	public void UseItem(int index)
	{
		if (index < 0 || index >= items.Count) return;
		Item item = items[index];
		IUsableItem usable = item as IUsableItem;  // IUsableItem 구현 확인
		if (usable != null)
		{
			usable.Use();  // 아이템 사용 로직 실행

			// 사용 후 소모 아이템이면 인벤토리에서 제거
			if (item.isConsumable)
			{
				items.RemoveAt(index);
			}
			UpdateUI();  // UI 갱신
		}
		else
		{
			Debug.Log("사용할 수 없는 아이템입니다.");
		}
	}

	// UI에 슬롯 업데이트 요청
	public void UpdateUI()
	{
		if (inventoryUI != null)
			inventoryUI.RefreshSlots(items);
	}
	
	public void MoveItem(int from, int to)
	{
		if (from < 0 || to < 0 || from >= items.Count || to >= items.Count) return;
		Item temp = items[from];
		items[from] = items[to];
		items[to] = temp;
		UpdateUI();
	}
}
