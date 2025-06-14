using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GadgetType { sword, wand, bow, armor, GadgetRemove }

public class GadgetEffectHandler : MonoBehaviour
{
    [Header("최대 장착 개수")]
    public int maxEquipCount = 3;

    [Header("Inventory 참조")]
    public Inventory inventory;
    private PlayerStat playerStat;
    private float baseAttackDamage, baseSkillDamage, baseAttackSpeed, baseMaxHP;

    // 1) 각 장비 적용 시 계산된 델타(변경량)만큼 보관할 레코드 클래스
    private class EffectRecord
    {
        public GadgetItem gadget;
        public int attackDelta;
        public int skillDelta;
        public float speedDelta;
        public int hpDelta;
    }
    // 실제 장착된 장비 효과 델타를 저장
    private List<EffectRecord> equippedEffects = new List<EffectRecord>();


    void Awake()
    {
        playerStat = GetComponent<PlayerStat>();
        baseAttackDamage = playerStat.attackDamage;
        baseSkillDamage = playerStat.skillDamage;
        baseAttackSpeed = playerStat.attackSpeed;
        baseMaxHP = playerStat.maxHP;
    }


    /// Inventory.UseItem()에서 호출됩니다.
    public bool TryUseGadget(GadgetItem gadget, int inventoryIndex)
    {
        if (gadget.gadgetType == GadgetType.GadgetRemove)
        {
            if (equippedEffects.Count == 0) return false;
            RemoveAllGadgets();
            inventory.RemoveItem(inventoryIndex);
            return true;
        }

        if (equippedEffects.Count >= maxEquipCount) return false;

        if (equippedEffects.Exists(r => r.gadget == gadget))
            return false;

        EffectRecord rec = ApplyEffectRecord(gadget);
        equippedEffects.Add(rec);
        inventory.RemoveItem(inventoryIndex);
        return true;
    }

    // 각각 델타 계산해 기록하고 스탯에 더해주는 메서드 
    private EffectRecord ApplyEffectRecord(GadgetItem g)
    {
        float mul = (g.gadgetRank == 1 ? 0.2f :
                     g.gadgetRank == 2 ? 0.5f : 1.2f);

        var rec = new EffectRecord { gadget = g };

        switch (g.gadgetType)
        {
            case GadgetType.sword:
                rec.attackDelta = Mathf.RoundToInt(baseAttackDamage * mul);
                playerStat.attackDamage += rec.attackDelta;
                break;

            case GadgetType.wand:
                rec.skillDelta = Mathf.RoundToInt(baseSkillDamage * mul);
                playerStat.skillDamage += rec.skillDelta;
                break;

            case GadgetType.bow:
                rec.speedDelta = baseAttackSpeed * mul;
                playerStat.attackSpeed += rec.speedDelta;
                break;

            case GadgetType.armor:
                rec.hpDelta = Mathf.RoundToInt(baseMaxHP * mul);
                playerStat.maxHP += rec.hpDelta;
                playerStat.currentHP = Mathf.Min(playerStat.currentHP + rec.hpDelta, playerStat.maxHP);
                break;
        }

        return rec;
    }

    private void RemoveAllGadgets()
    {
        // 제거 직전에 레벨업된 순수 스탯 기준 갱신
        UpdateBaseStats();


        // 기록된 델타만큼 스탯에서 빼기
        foreach (var rec in equippedEffects)
            RemoveEffectRecord(rec);

        // 인벤토리에 원복
        foreach (var rec in new List<EffectRecord>(equippedEffects))
            inventory.AddItem(rec.gadget);

        // 기록 초기화
        equippedEffects.Clear();

        // UI 초기화
        GadgetUI gui = FindObjectOfType<GadgetUI>();
        if (gui != null) gui.ClearAllSlots();
    }

    /// 현재 playerStat 값을 baseAttackDamage 등으로 덮어씌우는 메서드
    public void UpdateBaseStats()
    {
        baseAttackDamage = playerStat.attackDamage;
        baseSkillDamage  = playerStat.skillDamage;
        baseAttackSpeed  = playerStat.attackSpeed;
        baseMaxHP        = playerStat.maxHP;
    }

    private void RemoveEffectRecord(EffectRecord rec)
    {
        switch (rec.gadget.gadgetType)
        {
            case GadgetType.sword:
                playerStat.attackDamage -= rec.attackDelta;
                break;
            case GadgetType.wand:
                playerStat.skillDamage -= rec.skillDelta;
                break;
            case GadgetType.bow:
                playerStat.attackSpeed -= rec.speedDelta;
                break;
            case GadgetType.armor:
                playerStat.maxHP -= rec.hpDelta;
                if (playerStat.currentHP > playerStat.maxHP)
                    playerStat.currentHP = playerStat.maxHP;
                break;
        }
    }

    /// 현재 장착된 Sword 계열 장비의 공격 배율(1 + 합산 버프)을 리턴
    /// 예: 1성 하나면 1 + 0.2 = 1.2, 3성 하나면 1 + 1.2 = 2.2
    public float GetAttackMultiplier()
    {
        float sum = 1f;
        foreach (var rec in equippedEffects)
        {
            // rec.gadget이 실제 Sword 아이템
            if (rec.gadget.gadgetType == GadgetType.sword)
            {
                float mul = (rec.gadget.gadgetRank == 1 ? 0.2f :
                             rec.gadget.gadgetRank == 2 ? 0.5f : 1.2f);
                sum += mul;
            }
        }
        return sum;
    }

    /// 장착된 Wand 장비들의 스킬데미지 배율(1 + 합산 버프)을 반환
    public float GetSkillMultiplier()
    {
        float sum = 1f;
        foreach (var rec in equippedEffects)
        {
            if (rec.gadget.gadgetType == GadgetType.wand)
            {
                float mul = (rec.gadget.gadgetRank == 1 ? 0.2f :
                             rec.gadget.gadgetRank == 2 ? 0.5f : 1.2f);
                sum += mul;
            }
        }
        return sum;
    }

    /// <summary>장착된 Bow 장비들의 공격속도 배율(1 + 합산 버프) 반환</summary>
    public float GetSpeedMultiplier()
    {
        float sum = 1f;
        foreach (var rec in equippedEffects)
        {
            if (rec.gadget.gadgetType == GadgetType.bow)
            {
                float mul = (rec.gadget.gadgetRank == 1 ? 0.2f :
                             rec.gadget.gadgetRank == 2 ? 0.5f : 1.2f);
                sum += mul;
            }
        }
        return sum;
    }

    /// <summary>장착된 Armor 장비들의 HP 배율(1 + 합산 버프) 반환</summary>
    public float GetHPMultiplier()
    {
        float sum = 1f;
        foreach (var rec in equippedEffects)
        {
            if (rec.gadget.gadgetType == GadgetType.armor)
            {
                float mul = (rec.gadget.gadgetRank == 1 ? 0.2f :
                             rec.gadget.gadgetRank == 2 ? 0.5f : 1.2f);
                sum += mul;
            }
        }
        return sum;
    }
}