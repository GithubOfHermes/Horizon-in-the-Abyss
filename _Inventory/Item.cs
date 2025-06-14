using System.Collections.Generic;
using UnityEngine;

// Item.cs - IUsableItem 인터페이스는 Inventory.cs에 정의되어 있습니다.
[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject, IUsableItem
{
    [Header("아이템 기본 정보")]
    public string itemName;       // 아이템 이름
    public Sprite icon;           // 인벤토리 슬롯에 표시할 아이콘 이미지
    public bool isConsumable;     // 소비형 아이템 여부

    // 아이템 사용 시 동작
    public virtual void Use()
    {
        Debug.Log(itemName + "을(를) 사용했습니다.");
    }
}