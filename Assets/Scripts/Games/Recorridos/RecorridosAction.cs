using Assets.Scripts.Games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorridosAction : MonoBehaviour {

    public enum ActionToDo { Up, Down, Left, Right, Remove,Start}

    public ActionToDo currentAction;
    public Sprite sprite;

    public int indexInList;
 

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoAction()
    {
        RecorridosController.instance.AddAction(this);
    }



}
