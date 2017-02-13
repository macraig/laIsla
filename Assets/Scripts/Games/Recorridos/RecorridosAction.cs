using Assets.Scripts.Games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Sound;
namespace Assets.Scripts.Games.Recorridos
{
public class RecorridosAction : MonoBehaviour {

    public enum ActionToDo { Up, Down, Left, Right, Remove,Start}

    public ActionToDo currentAction;
    public Sprite sprite;

    public int indexInList;
 
    public void DoAction()
		{
			SoundController.GetController ().PlayClickSound ();
//			RecorridosController.instance.AddAction(this);
    }



}
}