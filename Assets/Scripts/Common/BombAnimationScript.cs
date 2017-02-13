using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games.Recorridos;
using UnityEngine;

namespace Assets.Scripts.Common
{

public class BombAnimationScript : MonoBehaviour {

		public RecorridosController controller;

		public void OnAnimationEnd()
		{
			gameObject.SetActive(false);
			controller.OnBombAnimationEnd();
		}

		public void ShowAnimation()
		{
			gameObject.SetActive(true);

		}
}
}
