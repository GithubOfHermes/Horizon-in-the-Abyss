using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ItemSlotUI.cs - 인벤토리 슬롯 하나당 이미지/텍스트 표시 및 클릭 처리
public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 컴포넌트 연결")]
    public Image iconImage;  // 아이콘 표시용 Image
    public Text nameText;    // 아이템 이름 표시용 Text

    // 슬롯 정보 프로퍼티
    public int SlotIndex { get; private set; }
    public Inventory Inventory { get; private set; }
    public Item Item { get; private set; }

    /// <summary>
    /// 슬롯에 아이템 데이터를 설정
    /// </summary>
    public void Setup(Item item, int idx, Inventory inv)
    {
        SlotIndex = idx;
        Inventory = inv;
        Item = item;

        iconImage.sprite = item.icon;
        iconImage.enabled = (item.icon != null); // 아이콘이 있다면 반드시 표시되도록

        nameText.text = item.itemName;
        nameText.enabled = true;
    }

    /// <summary>
    /// 빈 슬롯으로 초기화
    /// </summary>
    public void Clear()
    {
        iconImage.sprite = null;
        nameText.text = string.Empty;
        Inventory = null;
        Item = null;
    }

    /// <summary>
    /// 슬롯 클릭 시 아이템 사용 호출
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Inventory != null)
        {
            Inventory.UseItem(SlotIndex);
        }
    }
}
