using System;
using UnityEngine;
using Assets.Scripts.Sound;
using Assets.Scripts.Metrics.Model;
using Assets.Scripts.Metrics;
using Assets.Scripts.Settings;
using System.Collections.Generic;

namespace Assets.Scripts.App
{
    public class AppController : MonoBehaviour
    {
        private static AppController appController;
        private AppModel appModel;
		[SerializeField]
		private MetricsController metricsController;
        [SerializeField]
        private List<Game> games;


        void Awake(){
            if (appController == null) appController = this;
            else if (appController != this) Destroy(gameObject);     
            DontDestroyOnLoad(gameObject);
            appModel = new AppModel();

            
        }



		public GameMetrics GetCurrentMetrics()
        {
            return MetricsController.GetController().GetLastMetricOf(appModel.GetCurrentGame());
        }

		public Game GetGameById (int idGame)
		{
			throw new NotImplementedException ();
		}

		public MetricsController GetMetricsController(){
			return metricsController;
		}

     


        internal string GetGameName(int idGame)
        {
            for (int i = 0; i < games.Count; i++)
            {
				if (games[i].GetId() == idGame) return games[i].GetName();

            }
            return "Error";
        }

      

        internal void PlayCurrentGame()
        {
//            SoundController.GetController().StopMusic();
            // ViewController.GetController().StartGame(appModel.GetCurrentArea(), appModel.GetCurrentGame());
			SoundController.GetController ().StopMusic();
			ViewController.GetController().StartGame(GetCurrentGame());
      
        }

		internal void PlayNextGame()
		{
			ViewController.GetController().StartGame(GetNextGame());

		}


        internal void BackToGame()
        {
            Timer.GetTimer().Resume();
        }

        internal List<Game> GetGames()
        {
            return games;
        }

        internal void SetCurrentArea(int area)
        {
            appModel.SetCurrentArea(area);
        }

        internal void SetCurrentGame(Game currentGame){
            appModel.SetCurrentGame(currentGame);
        }

        public int GetCurrentLevel()
        {
            return appModel.GetCurrentLevel();
        }

        internal Game GetCurrentGame(){
			return appModel.GetCurrentGame ();
        }

		internal Game GetNextGame(){
			return appModel.GetNextGame ();
		}

        internal void ShowInGameMenu(){
            Timer.GetTimer().Pause();
            ViewController.GetController().ShowInGameMenu();
        }
        /*
        internal string GetActivityName(int area, int game)
        {
            return appModel.GetGameNames()[area][game][SettingsController.GetController().GetLanguage()];
        }
        */

        public static AppController GetController()
        {
            return appController;
        }

        internal int GetCurrentArea()
        {
            return appModel.GetCurrentArea();
        }

		public AppModel GetAppModel(){
			return appModel;
		}

        public void SetCurrentLevel(int level)
        {
            appModel.SetCurrentLevel(level);
        }
    }
}
