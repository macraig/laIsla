using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Sound;
using UnityEngine.UI;

namespace Assets.Scripts.Games.RompecabezasActivity {
	public class Part : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
		public static Part itemBeingDragged;
		private Vector3 startPosition;
		public bool active;
		private PartModel model;

		public void SetActive(bool isActive){
			active = isActive;
		}

		public void OnBeginDrag(PointerEventData eventData) {
			if (active) {
				SoundController.GetController().PlayDragSound();
				itemBeingDragged = this;
				startPosition = transform.position;
				GetComponent<CanvasGroup> ().blocksRaycasts = false;
			}
		}

		public void OnDrag(PointerEventData eventData) {
			if (active) 
				transform.position = Input.mousePosition;
		}

		public void OnEndDrag(PointerEventData eventData = null) {
			if (active) {
				SoundController.GetController().PlayDropSound();
				itemBeingDragged = null;
				GetComponent<CanvasGroup> ().blocksRaycasts = true;

				transform.position = startPosition;
			}
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