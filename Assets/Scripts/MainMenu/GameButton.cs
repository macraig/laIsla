using Assets.Scripts.Settings;
using Assets.Scripts.Sound;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.App;

namespace Assets.Scripts.MainMenu
{

    public class GameButton : MonoBehaviour
    {

//        public Text gameName;
        private Game data;

		/*IMORTANT: id MUST MATCH GAME ID IN games JSON*/
		public void OnClickGame(int id)
        {
            ClickSound();
            MainMenuController.GetController().ShowPreviewGame(id);
			AppController.GetController ().SetCurrentLevel (id);
        }

        private void ClickSound()
        {
            SoundController.GetController().PlayClickSound();
        }

        internal void SetAttributes(Game game, Color color)
        {
            this.data = game;
//            gameName.text = game.GetNames()[SettingsController.GetController().GetLanguage()];
            gameObject.GetComponent<Image>().color = color;
        }

//        public Text GetGameNameText()
//        {
//            return gameName;
//        }

        
    }
}