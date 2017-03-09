using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.MainMenu
{
public class IncorrectCodeScript : MonoBehaviour {

	

	public void OnAnimationEnd()
	{
		gameObject.SetActive(false);

	}

	internal void ShowAnimation()
	{
		gameObject.SetActive(true);

	}

	}
}