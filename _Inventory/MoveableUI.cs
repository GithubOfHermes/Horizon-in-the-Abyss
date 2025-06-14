using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class MoveableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas        canvas;
    private RectTransform rectTransform;
    private CanvasGroup   canvasGroup;
    private Transform     originalParent;
    private int           originalIndex;
    private Vector3       originalScale;

    private bool          isDragging;
    private bool          isGadgetDrag;     // GadgetItem 드래그인지 확인용

    private int        fromSlotIndex;
    private Inventory  inventory;
    private InventoryUI inventoryUI;

    void Awake()
    {
        // 자신이 속한 Canvas, InventoryUI 참조 자동 획득
        canvas        = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup   = GetComponent<CanvasGroup>();
        inventoryUI   = FindObjectOfType<InventoryUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 빈 슬롯(아이템 없음) 드래그 방지
        ItemSlotUI slotUI = GetComponent<ItemSlotUI>();
        if (slotUI == null || slotUI.Item == null)
            return;

        isDragging    = true;
        // GadgetItem이면 true
        isGadgetDrag  = (slotUI.Item is GadgetItem);

        originalParent = transform.parent;
        originalIndex  = transform.GetSiblingIndex();
        originalScale  = transform.localScale;

        fromSlotIndex = slotUI.SlotIndex;
        inventory     = slotUI.Inventory;

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        ItemSlotUI slotUI = GetComponent<ItemSlotUI>();

        // 합성이나 사용으로 inventory에서 제거된 아이템일 경우, 오브젝트 삭제
        if (slotUI != null && !inventory.items.Contains(slotUI.Item))
        {
            Destroy(gameObject);  // 드래그 오브젝트 삭제
            return;
        }

        // GadgetItem 드래그일 땐 이동 로직 없이 바로 복귀
        if (isGadgetDrag)
        {
            Restore();
            return;
        }

        // 일반 아이템만 슬롯 간 이동
        if (inventory != null)
        {
            GameObject target = eventData.pointerCurrentRaycast.gameObject;
            if (target != null)
            {
                ItemSlotUI toSlot = target.GetComponentInParent<ItemSlotUI>();
                if (toSlot != null)
                {
                    inventory.MoveItem(fromSlotIndex, toSlot.SlotIndex);
                    inventoryUI.RefreshSlots(inventory.items);
                    Restore();
                    return;
                }
            }
        }

        // 그 외: 원위치 복귀
        Restore();
    }

    private void Restore()
    {
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalIndex);
        transform.localScale = originalScale;
    }
}
