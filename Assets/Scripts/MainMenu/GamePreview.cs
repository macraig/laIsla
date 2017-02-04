using System;
using Assets.Scripts.Settings;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Sound;
using System.Collections.Generic;
using Assets.Scripts.App;

namespace Assets.Scripts.MainMenu
{

    public class GamePreview : MonoBehaviour
    {

        public Text gameTitle;
 	    public Text gameDescription;
        [SerializeField]
		private Image gameIcon;
        [SerializeField]
        private Button playButton;
		[SerializeField]
		private Text gameId;
   


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickBackBtn();
            }
        }

     
        internal void SetGameDescription(string description)
        {
            this.gameDescription.text = description.Replace("/n", "\n");
        }

		internal void SetGameNumber(string gameId)
		{
			this.gameId.text = gameId;
		}

		internal void SetTitle(string title)
		{
			this.gameTitle.text = title;
		}


     
        internal void SetIcon(Sprite gameSprite)
        {
			gameIcon.sprite = gameSprite;      
        }

        public void OnClickPlayBtn()
        {
            ClickSound();
          
			AppController.GetController().PlayCurrentGame();
        }

        public void OnClickBackBtn()
        {
            ViewController.GetController().SetCanvasScaler(0.5f);
            ClickSound();
            gameObject.SetActive(false);
        }

        

        internal void ClickSound()
        {
            SoundController.GetController().PlayClickSound();
        }

        public void SetInstruction(GameObject[] instructionsSequence)
        {
            throw new NotImplementedException();
        }
    }
}
