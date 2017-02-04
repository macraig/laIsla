using System;
using Assets.Scripts.Settings;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Cover {

    public class CoverScreen : MonoBehaviour {

        public CoverView coverView;

        public Text startText;
        public Text aboutText;

        public Toggle SpanishToggle;
        public Toggle EnglishToggle;

        public Toggle MusicToggle;
        public Toggle SoundToggle;

        void OnEnable()
        {
            UpdateTexts();
            if (SettingsController.GetController().GetLanguage() == 0)
            {
                SpanishToggle.isOn = true;
            }

            else
            {
                EnglishToggle.isOn = true;
            }
        }

        public void OnClickStartBtn()
        {
            coverView.ClickSound();
            CoverController.GetController().StartGame();
        }

        public void OnClickOxBtn(){
            coverView.ClickSound();
            coverView.ShowOx();
            gameObject.SetActive(false);
        }   

        public void OnClickAboutBtn(){
            coverView.ClickSound();
            coverView.ShowAbout();
            gameObject.SetActive(false);
        }       

        public void OnClickArgentineBtn()
        {
            SettingsController.GetController().SwitchLanguage(0);
            UpdateTexts();
        }    

        public void OnClickBritishBtn()
        {
            SettingsController.GetController().SwitchLanguage(1);
            UpdateTexts();
        }

        public void OnClickMusicToggle()
        {
            SettingsController.GetController().ToggleMusic();
        }

        public void OnClickSfxToggle()
        {
            SettingsController.GetController().ToggleSFX();
        }

        public void OnClickToggle()
        {
            SoundController.GetController().PlaySwitchSound();
        }

        private void UpdateTexts()
        {
            switch (SettingsController.GetController().GetLanguage())
            {
                case 0:
                    startText.text = "JUGAR";
                    aboutText.text = "Acerca de Calculandox";
                    break;
                default:
                    startText.text = "PLAY";
                    aboutText.text = "About Calculandox";
                    break;
            }
        }

        public void ClickSound()
        {
            coverView.ClickSound();
        }


    }
}
