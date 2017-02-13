using System;
using Assets.Scripts.Sound;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.Games.PuntosCardinalesActivity {
	public class PuntosCardinalesDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
		public static PuntosCardinalesDragger itemBeingDragged;
		public PuntosCardinalesActivityView view;
		public Vector3 originPosition;
		private Vector3 newPosition;
		public bool active,first = true;

		public void SetActive(bool isActive){
			active = isActive;
		}


		public void OnBeginDrag(PointerEventData eventData) {
			if (active) {
				SoundController.GetController ().SetConcatenatingAudios (false);
				view.soundBtn.interactable = true;
				SoundController.GetController().PlayDragSound ();
				itemBeingDragged = this;
				if (first) {
					originPosition = transform.position;
					first = false;
				}

				newPosition = transform.position;
				GetComponent<CanvasGroup> ().blocksRaycasts = false;
			}
		}

		public void OnDrag(PointerEventData eventData) {
			if (active) 
				transform.position = Input.mousePosition;
		}

		public void OnEndDrag(PointerEventData eventData = null) {
			if (active) {
				SoundController.GetController().PlayDropSound ();
				itemBeingDragged = null;
				GetComponent<CanvasGroup> ().blocksRaycasts = true;
				transform.position = newPosition;
//				view.ActivateDraggers (false);
			}

		}

		public void SetPosition (Vector3 position)
		{
			newPosition = position;
		}


		public void ReturnToOriginalPosition(){
//			transform.parent = GameObject.Find("buildingsPlaca").transform;
			transform.position = originPosition;
		}

		public bool WasDragged(){
			return !first;
		}
	}
}