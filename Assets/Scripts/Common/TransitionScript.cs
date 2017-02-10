using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;

namespace Assets.Scripts.Common
{

public class TransitionScript : MonoBehaviour {

		public LevelView levelView;

		public void OnAnimationEnd()
		{
			gameObject.SetActive(false);
			levelView.OnNextLevelAnimationEnd();
		}

		public void ShowAnimation()
		{
			gameObject.SetActive(true);

		}
}
}
