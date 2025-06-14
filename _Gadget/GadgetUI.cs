using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GadgetUI : MonoBehaviour, IDropHandler
{
    [Header("장착 슬롯 이미지 리스트")]
    public List<Image> slotImages;
    private List<Sprite> defaultBackgrounds;
    private List<GadgetItem> equippedGadgets = new List<GadgetItem>();
    private Inventory inventory;

    // Awake()로 Inspector에 지정된 Background 스프라이트를 미리 저장
    void Awake()
    {
        defaultBackgrounds = new List<Sprite>();
        foreach (var img in slotImages)
        {
            defaultBackgrounds.Add(img.sprite);
            // Inspector 창에서 지정해 둔 Background(Sprite)를 저장
        }
        inventory = FindObjectOfType<Inventory>();  
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        if (dragged == null) return;

        var slotUI = dragged.GetComponent<ItemSlotUI>();
        if (slotUI == null) return;

        var gadget = slotUI.Item as GadgetItem;
        if (gadget == null)
        {
            Debug.Log("Gadget이 아니거나 제거 장비이므로 무시");
            return;
        }

        if (gadget.gadgetType == GadgetType.GadgetRemove && equippedGadgets.Count == 0)
        {
            Debug.Log("장착된 가젯이 없어 제거 장비를 사용할 수 없습니다.");
            return;
        }

        if (gadget.gadgetType == GadgetType.GadgetRemove)
        {
            // 수정: 해체 로직 호출 (기존 장착 해제 및 스탯 복원)
            FindObjectOfType<GadgetEffectHandler>()
                .TryUseGadget(gadget, slotUI.SlotIndex);
            Debug.Log("장착된 가젯을 해체하고 인벤토리로 복귀했습니다.");  // 수정: 제거 후 로그만 출력
            return;  // 수정: 여기서 리턴해 UI 이미지 드롭 방지
        }

        if (gadget.gadgetType != GadgetType.GadgetRemove && equippedGadgets.Count >= slotImages.Count)
        {
            Debug.Log("장착 가능한 슬롯이 가득 찼습니다.");
            return;
        }

        var effectHandler = FindObjectOfType<GadgetEffectHandler>();              // 수정: 핸들러 참조
        bool success = effectHandler.TryUseGadget(gadget, slotUI.SlotIndex);      // 수정: 장착 시도 (스탯 증가 로직)
        if (!success)                                                            // 수정: 실패 시 UI 갱신 중단
        {
            Debug.Log("이미 같은 장비를 장착했거나 슬롯이 가득 찼습니다.");
            return;                                                              // 수정: 여기서 리턴하여 이미지만 뜨는 문제 방지
        }

        int idx = equippedGadgets.Count;
        slotImages[idx].sprite = gadget.icon;
        slotImages[idx].enabled = true;
        equippedGadgets.Add(gadget);

        Debug.Log("장착 완료: " + gadget.itemName);
    }

    public void ClearAllSlots()
    {
        for (int i = 0; i < slotImages.Count; i++)
        {
            // 아이콘만 지우는 대신, 기본 Background 스프라이트로 복구
            slotImages[i].sprite  = defaultBackgrounds[i];  
            slotImages[i].enabled = true;                   
        }
        equippedGadgets.Clear();  // 내부 리스트도 비워 줍니다
    }
}
