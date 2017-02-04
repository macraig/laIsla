using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

//EXCLUSIVE ABC ORDER. CANT MOVE IT, DONT USE, FUCKIN MONODEVELOP!
using UnityEngine.UI;


public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;
	Transform beginParent;


    public void OnBeginDrag(PointerEventData eventData) {
		GetComponent <Image>().color = Color.white;
		transform.SetParent (beginParent);
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
		itemBeingDragged = null;
		GetComponent<CanvasGroup>().blocksRaycasts = true;

		if(startParent != transform.parent){
			transform.position = transform.parent.position;
		}
    }

	public void BeginParent(Transform p){
		beginParent = p;
	}
}