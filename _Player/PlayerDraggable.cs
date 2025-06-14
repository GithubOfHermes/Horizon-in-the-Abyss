using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDraggable : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 offset;
    public GridManager gridManager;
    private Vector3 originalPosition;
    public Vector3 lastPosition;                                   // 수정: 마지막 유효 드롭 위치 저장
    public bool isDraggable = true;

    void Start()
    {
        mainCamera = Camera.main;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wall"), true);
        lastPosition = transform.position;                          // 수정: 시작 위치 초기화
    }

    void OnMouseDown()
    {
        if (!isDraggable) return;
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
        originalPosition = transform.position;
    }

    void OnMouseUp()
    {
        if (!isDraggable) return;
        isDragging = false;
        Vector3 snappedPos;
        if (gridManager.TryGetNearestGridPosition(transform.position, out snappedPos))
        {
            transform.position = snappedPos;
            lastPosition = snappedPos;                              // 수정: 유효 드롭 시 lastPosition 갱신
        }
        else
        {
            transform.position = originalPosition;
            Debug.Log("Dropped outside of grid. Reverting.");
        }
    }

    void Update()
    {
        if (!isDraggable || !isDragging) return;
        transform.position = GetMouseWorldPosition() + offset;
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
            return ray.GetPoint(rayDistance);
        return Vector3.zero;
    }
}