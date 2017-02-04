using UnityEngine;
using Assets.Scripts.Sound;
using Assets.Scripts.Metrics.Model;
using System;
using I18N;

namespace Assets.Scripts.Settings
{
    public class SettingsController : MonoBehaviour
    {

        private static SettingsController settingsController;
        private SettingsModel settingsModel;

        void Awake()
        {
            if (settingsController == null)
            {
                settingsController = this;
            }
            else if (settingsController != this)
            {
                Destroy(gameObject);
            }
            SystemLanguage systemLanguage = Application.systemLanguage;
           
            settingsModel = new SettingsModel(systemLanguage);
        }

        public void SwitchName(string newName)
        {
            settingsModel.SetUserName(newName);
            MetricsController.GetController().LoadFromDisk();
        }       

        public void ToggleMusic()
        {
            settingsModel.ToggleMusic();
//            if (!settingsModel.GetMusic()) SoundController.GetController().StopMusic();
//            else SoundController.GetController().PlayMusic();
        }

        internal bool GetMusic()
        {
            return settingsModel.GetMusic();
        }

        public void ToggleSFX()
        {
            settingsModel.ToggleSFX();
        }

        public void SwitchLanguage(int language)
        {
            settingsModel.SetLanguage(language);
			I18n.SetToCurrentLocale();
        }

        internal bool SaveUsername(string username)
        {
            if (username != "" && username.Length > 2)
            {
                SwitchName(username);
                return true;
            }
            return false;

        }

        public int GetLanguage() { return settingsModel.GetLangague(); }
        public string GetUsername() { return settingsModel.GetUserName(); }
        public bool MusicOn() { return settingsModel.GetMusic(); }
        public bool SfxOn() { return settingsModel.GetSfx(); }


        public static SettingsController GetController()
        {
            return settingsController;
        }
    }
}
