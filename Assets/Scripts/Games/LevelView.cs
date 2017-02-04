using Assets.Scripts.App;
using Assets.Scripts.Metrics;
using Assets.Scripts.Metrics.Model;
using Assets.Scripts.Sound;
using Assets.Scripts.Common;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Games
{
    // His childs have to implement a method called NextChallenge that
    // recieve whatever it needs
    public abstract class LevelView : MonoBehaviour
    {
      
		//Ingame Menu Panel
		public GameObject menuPanel;
		public Button menuBtn;
		//Explanation Panel
		public GameObject explanationPanel;
		public AudioClip explanationSound;
		//Right and wrong animations
		public Image rightAnimation;
		public Image wrongAnimation;
		//First turn
		protected bool first = true;


		// This method is used as the game's loop
		abstract public void Next(bool first = false);


		/*-----Functions for menuPanel panel-----*/

		public void OnClickMenuBtn(){
			PlaySoundClick();
			ShowInGameMenu();
		}

		virtual public void ShowInGameMenu(){
			menuPanel.transform.SetAsLastSibling ();
			menuPanel.SetActive (true);
		}

		public void HideInGameMenu(){
			PlaySoundClick ();
			menuPanel.SetActive (false);
		}

		public void OnClickInstructionsButton(){
			PlaySoundClick ();
			HideInGameMenu ();
			ShowExplanation ();
		}

		public void OnHoverInstructionsButton(){
			menuPanel.GetComponentInChildren<Text> ().text = "CONSIGNA";
		}

		public void OnHoverRestartButton(){
			menuPanel.GetComponentInChildren<Text> ().text = "VOLVER A JUGAR";
		}

		public void OnHoverQuitButton(){
			menuPanel.GetComponentInChildren<Text> ().text = "VOLVER AL MENÚ";
		}

		public void OnExitHover(){
			menuPanel.GetComponentInChildren<Text> ().text = "";
		}

		public void OnClickRestartButton(){
			PlaySoundClick ();
			RestartGame ();
		}

		public void OnClickExitGameButton(){
			PlaySoundClick ();
//			HideInGameMenu ();
			ExitGame ();
		}
			
		internal void ShowExplanation(){
			explanationPanel.transform.SetAsLastSibling ();
			explanationPanel.SetActive(true);
			SoundController.GetController ().PlayClip (explanationSound);
		}


		virtual public void HideExplanation(){
			PlaySoundClick ();
			explanationPanel.SetActive (false);
			if (first) {
				Next (true);
				first = false;
			}
		}


		internal void ExitGame(){
//			MetricsController.GetController().DiscardCurrentMetrics();
			ViewController.GetController().LoadMainMenu();
			SoundController.GetController ().PlayMusic ();
		}

        // This method have to restart the view of the game to the initial state
		virtual public  void RestartGame(){
			HideInGameMenu ();
			first = true;

		}

        // This method have to be called when the user clicks menuButton
        public void OnClickSurrender()
        {
            PlaySoundClick();
//            LevelController.GetLevelController().ResolveExercise();
            MetricsController.GetController().OnSurrender();
        }
        // This method have to be called when the user clicks a button
        internal void PlaySoundClick()
        {
            SoundController.GetController().PlayClickSound();
        }
        // This method have to be called when the answers is correct
        internal void PlayRightSound()
        {
            SoundController.GetController().PlayRightAnswerSound();
        }
        // This method have to be called when the answers is incorrect
        internal void PlayWrongSound()
        {
            SoundController.GetController().PlayFailureSound();
        }

		internal void PlayDropSound()
		{
			SoundController.GetController().PlayDropSound();
		}

		internal void PlayDragSound()
		{
			SoundController.GetController().PlayDragSound();
		}


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { OnClickMenuBtn(); }
        }


		public void EndGame(int minSeconds, int pointsPerSecond, int pointsPerError){
			MetricsController.GetController().GameFinished(minSeconds, pointsPerSecond, pointsPerError);
//			ShowEndPanel ();
			ViewController.GetController ().LoadEndPanel ();
		}

		internal void ShowRightAnswerAnimation(){
			rightAnimation.transform.SetAsLastSibling ();
			rightAnimation.GetComponent<AnswerAnimationScript>().ShowAnimation();
			SoundController.GetController ().PlayRightAnswerSound ();
		}

		internal void ShowWrongAnswerAnimation(){
			wrongAnimation.transform.SetAsLastSibling ();
			wrongAnimation.GetComponent<AnswerAnimationScript>().ShowAnimation();
			SoundController.GetController ().PlayFailureSound ();
		}

		virtual public void OnRightAnimationEnd(){
			EnableComponents (true);
			Next ();
		}

		virtual public void OnWrongAnimationEnd(){
			EnableComponents (true);
		}

		//Override to deactivate or activate components before and after right/wrong animations
		virtual public void EnableComponents(bool enable){
			menuBtn.interactable = enable;
//			soundBtn.interactable = enable;
		}


    }
}
