using Assets.Scripts.App;
using Assets.Scripts.Sound;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Settings
{

    public class SwitchPlayerView : MonoBehaviour
    {

        public SettingsView settingsView;

        public Text title;
        public Text incorrectInput;
        public InputField inputName;
        public Button ticBtn;

        void OnEnable()
        {
            incorrectInput.gameObject.SetActive(false);
            UpdateTexts();
            Debug.Log(SettingsController.GetController().GetUsername());
            Debug.Log(inputName.text);
            inputName.text = SettingsController.GetController().GetUsername();
        }

        private void UpdateTexts()
        {
            switch (SettingsController.GetController().GetLanguage())
            {
                case 0:
                    title.text = "CAMBIAR USUARIO";
                    inputName.placeholder.GetComponent<Text>().text = "Ingresa tu nombre";
                    incorrectInput.text = "Por favor, ingresa tu nombre";

                    break;
                default:
                    title.text = "SWITCH USER";
                    inputName.placeholder.GetComponent<Text>().text = "Insert your name";
                    incorrectInput.text = "Please, insert your name";
                    break;
            }
        }

        public void OnClickTicBtn()
        {
            PlayClickSound();
            CheckEnteredUsername();
        }

       
        void CheckEnteredUsername()
        {
            inputName.text = inputName.text.Trim();
            if (SettingsController.GetController().SaveUsername(inputName.text.ToLower())) {
                settingsView.ShowGeneralSettings();
                gameObject.SetActive(false);
            } else ShowIncorrectInputAnimation();           
        }

        internal void ShowIncorrectInputAnimation()
        {
            ticBtn.interactable = false;
            ticBtn.enabled = false;
            incorrectInput.GetComponent<IncorrectUserAnimation>().ShowIncorrecrUserAnimation();
        }

        public void OnIncorrectInputAnimationEnd()
        {
            ticBtn.interactable = true;
            ticBtn.enabled = true;
        }

        public void OnClickCrossBtn()
        {
            PlayClickSound();
            settingsView.ShowGeneralSettings();
            gameObject.SetActive(false);
        }

        private void PlayClickSound()
        {
            SoundController.GetController().PlayClickSound();
        }

    }
}
