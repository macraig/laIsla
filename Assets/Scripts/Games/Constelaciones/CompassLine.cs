using System;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompassLine : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public void OnBeginDrag(PointerEventData eventData) { }

	public void OnDrag(PointerEventData eventData) {
		Vector3 dir = Input.mousePosition - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void OnEndDrag(PointerEventData eventData) { }

	public void UnFix() {
		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.rotation = Quaternion.identity;
	}

	public void ToggleCompassVisibility() {
		SoundController.GetController().PlayClickSound();
		gameObject.SetActive(!gameObject.activeSelf);
	}

	public static void SetPivot(RectTransform rectTransform, Vector2 pivot) {
		if (rectTransform == null) return;

		Vector2 size = rectTransform.rect.size;
		Vector2 deltaPivot = rectTransform.pivot - pivot;
		Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
		rectTransform.pivot = pivot;
		rectTransform.localPosition -= deltaPosition;
	}
}