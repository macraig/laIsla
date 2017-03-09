using Assets.Scripts.App;
using Assets.Scripts.Settings;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{



	public class BlockPanel : MonoBehaviour
	{
		public  const String PASSWORD = "extraclase";

		public List<Toggle> gameToggles;
		public InputField inputText;
		public Text incorrectCode;
		private List<bool> startStatus;

		public void OnCloseButtonClick(){
			inputText.text = "";
			this.gameObject.SetActive (false);
			for (int i = 0; i < gameToggles.Count; i++) {
				gameToggles [i].isOn = startStatus [i];
			}
				
		}

		public void SetStartState ()
		{
			startStatus = new List<bool> ();
			foreach (Toggle toggle in gameToggles) {
				startStatus.Add (toggle.isOn);
			}
		}

		public void OnOkButtonClick(){
			//Check password
			if (inputText.text.ToLower () == PASSWORD) {
				List<bool> gamesStatus = new List<bool> ();
				foreach (Toggle toggle in gameToggles) {
					gamesStatus.Add (toggle.isOn);
				}

				MainMenuController.GetController ().EnableGames (gamesStatus);
				inputText.text = "";
				this.gameObject.SetActive (false);
			} else {
				incorrectCode.GetComponent<IncorrectCodeScript>().ShowAnimation();
			}



		}




	}

}