using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// 방 종류 정의
public enum RoomType { Start, Normal, Clear, Shop, Elite, Boss }

// 각 방에 부착하여 플레이어 입장/퇴장을 감지하는 트리거 스크립트
[RequireComponent(typeof(Collider))]
public class RoomTrigger : MonoBehaviour
{
    [Header("이 방의 타입")]  
    public RoomType roomType;

    [HideInInspector]
    public bool isCleared = false; // 클리어 상태 유지용 플래그

    void Reset()
    {
        // 트리거 콜라이더 자동 설정
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StageManager.Instance.EnterRoom(this);
    }
}
