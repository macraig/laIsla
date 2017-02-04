using System;
using System.Collections.Generic;
using Assets.Scripts.Metrics.Model;
using SimpleJSON;
using UnityEngine;

namespace Assets.Scripts.App
{
    public class AppModel
    {
        private int currentArea;
        private Game currentGame;
        private int currentLevel;
		private List<int> gameOrder = new List<int>(){1,2,3,4,6,5,7};

		private List<Game> games; 
		private Sprite[] icons;

        public AppModel()
        {
			loadGames ();

        }

		void loadGames ()
		{

			JSONArray gamesObj = JSON.Parse(Resources.Load<TextAsset>("Jsons/games").text).AsObject["games"].AsArray;
			icons = Resources.LoadAll<Sprite>("Sprites/icons");

			games = new List<Game>();
			foreach(JSONClass game in gamesObj) {
				Game newGame = new Game ();
				newGame.SetId (int.Parse(game["id"].Value));
				newGame.SetName (game ["name"].Value);
				newGame.SetPrefabName (game ["prefabName"].Value);
				newGame.SetDescription( game ["description"].Value);
				newGame.SetIcon (icons[int.Parse(game ["icon"].Value)]);
				games.Add (newGame);
			}

			AppController.GetController ().GetMetricsController ().SetMetricsModel (new MetricsModel(games));
		}



		public Game GetGameById (int id)
		{
			for (int i = 0; i < games.Count; i++) {
				if (games [i].GetId () == id)
					return games [i];
			
			}
			Debug.Log ("NO GAME FOUND in AppModel.GetGameById");
			return null;

		}	

		public Game GetNextGame(){
			int gameIndex = gameOrder.IndexOf (currentGame.GetId());
			if (gameIndex < gameOrder.Count - 1) 
				currentGame = GetGameById(gameOrder[gameIndex + 1]);
			else 
				currentGame = GetGameById(gameOrder[0]);
			return currentGame;
			

		}

        public Game GetCurrentGame(){
            return currentGame;
        }

		public List<Game> GetGames(){
			return games;
		}

        internal void SetCurrentGame(Game currentGame){
            this.currentGame = currentGame;
        }

        internal int GetCurrentLevel()
        {
            return currentLevel;
        }

        internal void SetCurrentLevel(int level)
        {
            currentLevel = level;
        }

        internal int GetCurrentArea()
        {
            return currentArea;
        }

        internal void SetCurrentArea(int area)
        {
            this.currentArea = area;
        }

       
    }
}