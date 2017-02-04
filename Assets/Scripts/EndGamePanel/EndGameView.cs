using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.App;
using Assets.Scripts.Sound;
using UnityEngine.UI;
namespace Assets.Scripts.EndGamePanel{
public class EndGameView : MonoBehaviour {

		public GameObject starPanel;
		private Sprite star;


		void Start(){
			ShowEndPanel ();
		}

		public void ShowEndPanel(){
			SoundController.GetController ().PlayLevelCompleteSound ();
			ShowStars ();
		}

		void ShowStars ()
		{
			star = Resources.Load<Sprite> ("Sprites/star");
			int stars = AppController.GetController ().GetCurrentMetrics ().GetStars ();
			for(int i = 0; i < stars; i++)
			{            
				Image starImage = starPanel.GetComponentsInChildren<Image> (true) [i+1];
				starImage.sprite=star;
			}
		}

		internal void PlaySoundClick()
		{
			SoundController.GetController().PlayClickSound();
		}

		public void OnClickRestartButton(){
			PlaySoundClick ();
			RestartGame ();

		}

		public  void RestartGame(){
			ViewController.GetController ().RestartCurrentGame ();
		}

		public void OnClickExitGameButton(){
			PlaySoundClick ();
			ExitGame ();

		}

		public void OnClickNextGameButton(){
			PlaySoundClick ();
			NextGame ();

		}

		internal void ExitGame(){
			ViewController.GetController().LoadMainMenu();
			SoundController.GetController ().PlayMusic ();
		}

		internal void NextGame(){
			PlaySoundClick ();

			AppController.GetController().PlayNextGame();

		}

		public void OnEndHoverRestartButton(){
			gameObject.GetComponentInChildren<Text> ().text = "VOLVER A JUGAR";

		}

		public void OnEndHoverQuitButton(){
			gameObject.GetComponentInChildren<Text> ().text = "VOLVER AL MENÚ";

		}

		public void OnEndHoverNextButton(){
			gameObject.GetComponentInChildren<Text> ().text = "SIGUIENTE JUEGO";

		}

		public void OnEndExitHover(){
			gameObject.GetComponentInChildren<Text> ().text = "";
		}
}
}