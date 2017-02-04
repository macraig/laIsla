using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Sound;
using System;
using Assets.Scripts.Common;

namespace Assets.Scripts.Cover{

    public class CoverView : MonoBehaviour{      

        public GameObject coverScreen;
        public GameObject oxScreen;
        public GameObject aboutScreen;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (aboutScreen.activeSelf)
                {
                    ClickSound();
                    ShowCoverScreen();
                    aboutScreen.SetActive(false);
                } else if (oxScreen.activeSelf)
                {
                    ClickSound();
                    ShowCoverScreen();
                    oxScreen.SetActive(false);
                }
                else
                {
                    Utils.MinimizeApp();
                }
            }
        }

        internal void ShowCoverScreen(){
            coverScreen.SetActive(true);
        }     

		public void ShowOx()
        {
			ClickSound ();
			oxScreen.SetActive(true);
        }

		public void OnClickStartBtn()
		{
			SoundController.GetController ().PlayClickSound ();
			CoverController.GetController().StartGame();
		}

		public void ShowAbout()
        {
			ClickSound ();
			aboutScreen.SetActive(true);

        }

        internal void ClickSound()
        {
            SoundController.GetController().PlayClickSound();
        }
    }
}
