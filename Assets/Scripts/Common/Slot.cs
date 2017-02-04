using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
//using Assets.Scripts.Levels.AbcOrder;

//EXCLUSIVE ABC ORDER. CANT MOVE IT, DONT USE, FUCKIN MONODEVELOP!
public class Slot : MonoBehaviour, IDropHandler {
//	public AbcOrderView view;

    public GameObject item {
        get {
            if (transform.childCount == 1) {
				var obj = transform.GetChild (0);
				return obj.GetComponent<Text>() != null ? null : obj.gameObject;
			} else if(transform.childCount > 1){
				return transform.GetChild (1).gameObject;
			}
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData) {
        if (!item) {
			try{
		        DragHandler.itemBeingDragged.transform.SetParent(transform);
				DragHandler.itemBeingDragged.GetComponent<DragHandler>().OnEndDrag (null);
//				view.Try (GetComponent<Image>());
			} catch(NullReferenceException ignored) {}
        }
    }
}