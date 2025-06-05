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

	void Start()
	{
		mainCamera = Camera.main;
	}

	void OnMouseDown()
	{
		isDragging = true;
		offset = transform.position - GetMouseWorldPosition();
		originalPosition = transform.position; // 위치 저장
	}

	void OnMouseUp()
	{
		isDragging = false;

		Vector3 snappedPos;
		if (gridManager.TryGetNearestGridPosition(transform.position, out snappedPos))
		{
			transform.position = snappedPos;
		}
		else
		{
			transform.position = originalPosition; // 원래 자리로 돌아감
			Debug.Log("Dropped outside of grid. Reverting.");
		}
	}

	void Update()
	{
		if (isDragging)
		{
			Vector3 mousePos = GetMouseWorldPosition() + offset;
			transform.position = mousePos;
		}
	}

	Vector3 GetMouseWorldPosition()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // y = 0 평면
		float rayDistance;

		if (groundPlane.Raycast(ray, out rayDistance))
		{
			return ray.GetPoint(rayDistance);
		}

		return Vector3.zero; // 실패 시 기본값
	}	

}
