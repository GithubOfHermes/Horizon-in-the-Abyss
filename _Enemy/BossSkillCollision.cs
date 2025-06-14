using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillCollision : MonoBehaviour
{
    public int damage = 50;            // ■ 설정: 플레이어에 입힐 데미지 값

    private bool hasHit = false;        // ■ 추가: 중복 데미지 방지 플래그

    void OnEnable()
    {
        hasHit = false;                 // ■ 추가: 오브젝트 활성화 시 플래그 초기화
    }

    void OnTriggerEnter(Collider other)
    {
        // ■ 수정: Player 태그만 처리, 중복 방지
        if (hasHit || !other.CompareTag("Player"))
            return;

        hasHit = true;                  // ■ 수정: 첫 충돌 시 한 번만 처리

        // ■ 수정: PlayerStat 컴포넌트로 데미지 전달
        var ps = other.GetComponent<PlayerStat>();
        if (ps != null)
            ps.TakeDamage(damage);
    }
}
