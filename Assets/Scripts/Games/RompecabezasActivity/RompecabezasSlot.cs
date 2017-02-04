using System;
using UnityEngine.EventSystems;
using UnityEngine;
using Assets.Scripts.Sound;
using UnityEngine.UI;
using Assets.Scripts.Games.RompecabezasActivity;
using Assets.Scripts.Common;
using System.Collections.Generic;

public class RompecabezasSlot : MonoBehaviour, IDropHandler, IPointerClickHandler {
	public RompecabezasActivityView view;
	private Part current;
	private bool isEndSlot, isStartSlot;
	private Sprite startSprite;

	public void OnDrop(PointerEventData eventData) {
		Part target = Part.itemBeingDragged;
		if(target != null && !isStartSlot && !isEndSlot) {
			SoundController.GetController().PlayDropSound();

			if(current != null){
				current.gameObject.SetActive(true);
			}

			current = target;
			this.GetComponent<Image>().sprite = target.GetComponent<Image>().sprite;
			target.gameObject.SetActive(false);
			target.OnEndDrag();
		}
	}

	public void Start(){
		startSprite = GetComponent<Image>().sprite;
	}

	public Part GetCurrent(){
		return current;
	}

	public void EndSlot(bool s){
		isEndSlot = s;
	}

	public void StartSlot(bool b) {
		isStartSlot = b;
	}

	public void OnPointerClick(PointerEventData eventData) {
		if(current != null){
			current.gameObject.SetActive(true);
			current = null;
			ResetSprite();
		}
	}

	public void SetStart(PartModel start) {
		StartSlot(true);
		switch(start.direction) {
		case Direction.DOWN:
			GetComponent<Image>().sprite = view.PartSprite(3);
			break;
		case Direction.LEFT:
			GetComponent<Image>().sprite = view.PartSprite(0);
			break;
		case Direction.UP:
			GetComponent<Image>().sprite = view.PartSprite(2);
			break;
		case Direction.RIGHT:
			GetComponent<Image>().sprite = view.PartSprite(1);
			break;
		}
	}

	public void SetEnd(PartModel end) {
		EndSlot(true);
		switch(end.previousDir) {
		case Direction.DOWN:
			GetComponent<Image>().sprite = view.PartSprite(6);
			break;
		case Direction.LEFT:
			GetComponent<Image>().sprite = view.PartSprite(5);
			break;
		case Direction.UP:
			GetComponent<Image>().sprite = view.PartSprite(7);
			break;
		case Direction.RIGHT:
			GetComponent<Image>().sprite = view.PartSprite(4);
			break;
		}
	}

	public bool IsEnd() {
		return isEndSlot;
	}

	public void ResetSprite() {
		GetComponent<Image>().sprite = startSprite;
	}
}