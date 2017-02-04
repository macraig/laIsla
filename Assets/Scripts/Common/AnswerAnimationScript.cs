using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;

namespace Assets.Scripts.Common
{

public class AnswerAnimationScript : MonoBehaviour {

		public LevelView levelView;

		public void OnRightAnswerAnimationEnd()
		{
			gameObject.SetActive(false);
			levelView.OnRightAnimationEnd();
		}

		public void OnWrongAnswerAnimationEnd()
		{
			gameObject.SetActive(false);
			levelView.OnWrongAnimationEnd();
		}

		public void ShowAnimation()
		{
			gameObject.SetActive(true);

		}
}
}
