using System;
using UnityEngine;

namespace Assets.Scripts.Settings
{
    internal class SettingsModel
    {
        private string userName;
        private int language;
        private bool soundEffects;
        private bool music;

        public SettingsModel(SystemLanguage systemLanguage)
        {
            userName = "";
            language = systemLanguage == SystemLanguage.Spanish ? 0 : 1;
            soundEffects = true;
            music = true;
        }   

        internal void ToggleMusic()
        {
            music = !music;
        }

        internal void ToggleSFX()
        {
            soundEffects = !soundEffects;
        }

        internal void SetUserName(string newName)
        {
            this.userName = newName;
        }

        internal void SetLanguage(int language)
        {
            this.language = language;
        }

        internal int GetLangague()
        {
            return language;
        }        

        internal string GetUserName()
        {
            return userName;
        }

        internal bool GetSfx()
        {
            return soundEffects;
        }

        internal bool GetMusic()
        {
            return music;
        }
    }
}