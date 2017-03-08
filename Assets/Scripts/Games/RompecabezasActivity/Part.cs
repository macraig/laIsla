using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Sound;
using UnityEngine.UI;

namespace Assets.Scripts.Games.RompecabezasActivity {
	public class Part : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
		public static Part itemBeingDragged;
		private Vector3 originPosition;
		private Vector3 newPosition;
		public bool active,first;
		private PartModel model;
		private RompecabezasSlot myCurrentSlot;

		public void SetActive(bool isActive){
			active = isActive;
		}

		public void OnBeginDrag(PointerEventData eventData) {
			Debug.Log ("Begin drag");
			if (active) {
				Debug.Log ("active");
				SoundController.GetController().PlayDragSound();
				itemBeingDragged = this;

				if (first) {
					Debug.Log ("first");
					originPosition = transform.position;
					first = false;
				}

				newPosition = transform.position;
				GetComponent<CanvasGroup> ().blocksRaycasts = false;
			}
		}

		public void OnDrag(PointerEventData eventData ) {
			if (active) 
				transform.position = Input.mousePosition;
		}

		public void OnEndDrag(PointerEventData eventData = null) {
			Debug.Log ("endDrag");
			if (active) {
				Debug.Log ("endDragActive");
				SoundController.GetController().PlayDropSound();
				itemBeingDragged = null;
				GetComponent<CanvasGroup> ().blocksRaycasts = true;
				transform.position = newPosition;
			}
		}

		public void SetPosition (Vector3 position)
		{
			newPosition = position;
		}

		public void SetSlot (RompecabezasSlot slot)
		{
			myCurrentSlot = slot;
		}

		public RompecabezasSlot GetCurrentSlot ()
		{
			return myCurrentSlot;
		}

		public void ReturnToOriginalPosition(){
			transform.position = originPosition;
			myCurrentSlot = null;
		}

		public bool WasDragged(){
			return !first;
		}

		public PartModel Model(){
			return model;
		}

		public void SetModel(PartModel partModel, List<Sprite> parts) {
			model = partModel;

			this.GetComponent<Image>().sprite = partModel.GetSprite(parts);
		}
	}
}