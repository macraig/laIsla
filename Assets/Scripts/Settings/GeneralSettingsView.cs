using System;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Settings
{
    public class GeneralSettingsView : MonoBehaviour
    {

        public SettingsView settingsView;

        public Text title;
        public Text languageLabel;
        public Text musicLabel;
        public Text soundLabel;
        public Text switchPlayerLabel;
        public Text exitGameText;
        // 0 spanish, 1 english
        public List<Toggle> languageBtns;


        void OnEnable()
        {

            languageBtns[SettingsController.GetController().GetLanguage()].isOn = true; // toggle calls UpdateTexts(); when its value change

        }

        public void OnToggleMusic()
        {
            ClickSound();
            SettingsController.GetController().ToggleMusic();
        }

        public void OnToggleSound()
        {
            SettingsController.GetController().ToggleSFX();
            ClickSound();
        }

        public void OnToggleArgentine()
        {
            ClickSound();
            SettingsController.GetController().SwitchLanguage(0);
            UpdateTexts();
        }

        public void OnToggleBritish()
        {
            ClickSound();
            SettingsController.GetController().SwitchLanguage(1);
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            switch (SettingsController.GetController().GetLanguage())
            {
                case 0:
                    title.text = "CONFIGURACIÓN";
                    languageLabel.text = "IDIOMA";
                    musicLabel.text = "MÚSICA";
                    soundLabel.text = "SONIDO";
                    switchPlayerLabel.text = "CAMBIAR JUGADOR";
                    exitGameText.text = "Salir del juego";
                    break;
                default:
                    title.text = "SETTINGS";
                    languageLabel.text = "LANGUAGE";
                    musicLabel.text = "MUSIC";
                    soundLabel.text = "SOUND";
                    switchPlayerLabel.text = "SWITCH PLAYER";
                    exitGameText.text = "Exit game";
                    break;
            }
        }

        public void OnClickSwitchPlayer()
        {
            ClickSound();
            settingsView.ShowSwitchPlayer();
            gameObject.SetActive(false);
        }

        public void OnClickExitButton()
        {
            ClickSound();
            Application.Quit();
        }

        public void OnClickClose()
        {
            ClickSound();
            settingsView.CloseSettings();
        }

        private void ClickSound()
        {
            SoundController.GetController().PlayClickSound();
        }
    }
}
