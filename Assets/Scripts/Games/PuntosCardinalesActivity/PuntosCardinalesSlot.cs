using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Games.PuntosCardinalesActivity {
	public class PuntosCardinalesSlot : MonoBehaviour, IDropHandler {
		public PuntosCardinalesActivityView view;
		public int row, column;

		public void OnDrop(PointerEventData eventData) {
			PuntosCardinalesDragger target = PuntosCardinalesDragger.itemBeingDragged;
			if(target != null) {
				view.Dropped(target, this, row, column);
			}
		}
	}
}