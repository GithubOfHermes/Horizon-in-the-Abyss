using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryUI : MonoBehaviour
{
	[Header("UI 요소 연결")]
	public GameObject inventoryWindow;  // 인벤토리 전체 UI Panel
	public GameObject slotPrefab;       // 슬롯 프리팹 (ItemSlotUI가 붙어있어야 함)
	public Transform slotParent;        // 슬롯을 배치할 부모(예: GridLayoutGroup 오브젝트)

	private Inventory inventory;        // 연결된 Inventory 스크립트
	private bool isOpen = false;        // 창 활성화 여부

	// 인벤토리 초기화: Inventory 스크립트 참조하고 UI 갱신
	public void Initialize(Inventory inv)
	{
		inventory = inv;
		RefreshSlots(inventory.items);
	}

	// 버튼 클릭 등으로 UI 토글
	public void Toggle()
	{
		isOpen = !isOpen;
		inventoryWindow.SetActive(isOpen);
		if (isOpen)
		{
			RefreshSlots(inventory.items);
			// 추가로 한 번 더!
			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)slotParent);
		}
	}

	public void RefreshSlots(List<Item> items)
	{
		// 1) 기존 슬롯 전부 제거
		foreach (Transform child in slotParent)
			Destroy(child.gameObject);

		// 2) 새 슬롯 생성
		for (int i = 0; i < inventory.capacity; i++)
		{
			GameObject slotObj = Instantiate(slotPrefab, slotParent, false);
			ItemSlotUI slotUI = slotObj.GetComponent<ItemSlotUI>();
			if (i < items.Count) slotUI.Setup(items[i], i, inventory);
			else slotUI.Clear();
		}

		// 3) 레이아웃 강제 갱신
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)slotParent);
	}
}