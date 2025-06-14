using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 어트리뷰트가 있어야 Create → Inventory → GadgetItem 메뉴에 뜹니다.
[CreateAssetMenu(menuName = "Inventory/GadgetItem", fileName = "NewGadgetItem")]
public class GadgetItem : Item
{
    [Header("장비 타입 및 등급")]
    public GadgetType gadgetType;  // sword, wand, bow, armor, GadgetRemove

    [Tooltip("1성=1, 2성=2, 3성=3 (최대 3)")]
    public int gadgetRank;
}
