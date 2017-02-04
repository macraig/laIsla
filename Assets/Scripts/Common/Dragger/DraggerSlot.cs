using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.Dragger {
	public class DraggerSlot : MonoBehaviour, IDropHandler {
		public DraggerView view;

		public void OnDrop(PointerEventData eventData) {
			DraggerHandler target = DraggerHandler.itemBeingDragged;
			if(view.CanDropInSlot(target, this)){
				target.Dropped();
				target.OnEndDrag();
				view.Dropped(target, this);
			}
		}
	}
}