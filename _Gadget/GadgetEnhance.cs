using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GadgetEnhance : MonoBehaviour, IDropHandler
{
    private Inventory   inventory;
    private InventoryUI inventoryUI;

    void Awake()
    {
        inventory   = FindObjectOfType<Inventory>();
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;
        ItemSlotUI fromSlot = draggedObj.GetComponent<ItemSlotUI>();
        if (fromSlot == null) return;

        GameObject rayObj = eventData.pointerCurrentRaycast.gameObject;
        if (rayObj == null) return;
        ItemSlotUI toSlot = rayObj.GetComponentInParent<ItemSlotUI>();
        if (toSlot == null) return;

        int idxA = fromSlot.SlotIndex;
        int idxB = toSlot.SlotIndex;

        if (idxA < 0 || idxB < 0 ||
            idxA >= inventory.items.Count ||
            idxB >= inventory.items.Count)
            return;

        GadgetItem gA = fromSlot.Item as GadgetItem;
        GadgetItem gB = toSlot.Item   as GadgetItem;
        if (gA == null || gB == null) return;

        if (gA.gadgetType != gB.gadgetType ||
            gA.gadgetRank != gB.gadgetRank)
            return;

        if (gA.gadgetRank >= 3)
        {
            Debug.Log("최고 등급 장비는 합성할 수 없습니다.");
            return;
        }

        int max = Mathf.Max(idxA, idxB);
        int min = Mathf.Min(idxA, idxB);
        
        // 수정: Inventory.RemoveItem 호출 대신 직접 리스트에서 제거하여 대기 아이템 자동 추가 방지
        inventory.items.RemoveAt(max);   // 삭제 순서를 유지하기 위해 큰 인덱스 먼저 제거
        inventory.items.RemoveAt(min);   // 재료 아이템 삭제 시 빈 슬롯 생김

        // 새 아이템 생성
        GadgetItem newG = ScriptableObject.CreateInstance<GadgetItem>();
        int nextRank = gA.gadgetRank + 1;
        string spriteName = gA.gadgetType.ToString().ToLower() + "_" + nextRank;

        newG.icon = Resources.Load<Sprite>(spriteName);
        if (newG.icon == null)
            Debug.LogError("[GadgetEnhance] 스프라이트 로드 실패: Resources/" + spriteName);

        // 아이콘과 이름을 일치시키기 위해 itemName도 같은 형식으로
        newG.itemName = spriteName;
        newG.isConsumable = gA.isConsumable;
        newG.gadgetType   = gA.gadgetType;
        newG.gadgetRank   = nextRank;
        
        // Insert 하기 전에 capacity 초과 방지
        if (inventory.items.Count >= inventory.capacity)
        {
            Debug.Log("인벤토리에 빈 슬롯이 없어 합성 결과를 추가할 수 없습니다.");
            return;
        }

        // 제거된 슬롯 위치 중 하나(min)에 삽입
        inventory.items.Insert(min, newG);
        inventoryUI.RefreshSlots(inventory.items);

        // 합성 결과 아이템 추가 후 빈 슬롯만큼 대기 아이템 처리
        inventory.ProcessPendingItems();


        Debug.Log(gA.gadgetType + " 등급 " + newG.gadgetRank + " 합성 완료");
    }
}
