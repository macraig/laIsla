using Assets.Scripts.App;
using Assets.Scripts.Settings;
using System;
using System.Collections.Generic;
using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{

    public class MainMenuController : MonoBehaviour
    {
        private static MainMenuController mainMenuController;

        public MenuView menuView;
        public GamePreview gamePreview;
		public BlockPanel blockPanel;


        [SerializeField]
        // numbering, geometry, ability, data
       
        void Awake()
        {
            if (mainMenuController == null) mainMenuController = this;
            else if (mainMenuController != this) Destroy(gameObject);
        }

   
        public static MainMenuController GetController()
        {
            return mainMenuController;
        }

        internal void ShowMenu()
        {
            menuView.gameObject.SetActive(true);
        }

        
      
		internal void GoBack() {
			ViewController.GetController().LoadLogin();
		}

        internal void ShowPreviewGame(int id)
        {
            
            ViewController.GetController().SetCanvasScaler(1f);
			Game game = AppController.GetController ().GetAppModel ().GetGameById (id);
			AppController.GetController().SetCurrentGame(game);
			gamePreview.SetGameDescription(game.GetDescription());
            gamePreview.SetIcon(game.GetIcon());
			gamePreview.SetTitle(game.GetName());
//			gamePreview.SetGameNumber(game.GetId().ToString());
            gamePreview.gameObject.SetActive(true);
//            menuView.gameObject.SetActive(false);

//            foreach (Animator animator in InstructionController.GetController().Animators)
//            {
//                Destroy(animator.gameObject);
//            }
           
        }

		public void ShowBlockPanel()
		{
			blockPanel.gameObject.SetActive (true);
			blockPanel.SetStartState ();
		}

		public void EnableGames (List<bool> gamesStatus)
		{
			menuView.EnableGames (gamesStatus);
		}

        internal void ShowSettings()
        {
            ViewController.GetController().LoadSettings();
        }



       
    }
}