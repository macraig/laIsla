using System;
using Assets.Scripts.Metrics.Model;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Games.Constelaciones {
	public class ConstelacionesModel : LevelModel {
		private int currentLvl;
		List<ConstelacionesLevel> lvls;
		List<string> starLetters;

		public ConstelacionesModel() {
			currentLvl = 0;
			starLetters = new List<string> (){"A","B","C","D","E","F","G","H","I","J","K","L","M","N"};
			StartLevels();
			MetricsController.GetController().GameStart();
		}

		public bool GameEnded(){
			return currentLvl == lvls.Count;
		}

		void StartLevels() {
			lvls = new List<ConstelacionesLevel>();
			JSONArray lvlsJson = JSON.Parse(Resources.Load<TextAsset>("Jsons/ConstalacionesActivity/levels").text).AsObject["levels"].AsArray;
			foreach(JSONNode lvlJson in lvlsJson) {
				lvls.Add(new ConstelacionesLevel(lvlJson.AsObject));
			}

			//lvls = Randomizer.RandomizeList(lvls);
		}

		public ConstelacionesLevel CurrentLvl() {
			return lvls[currentLvl];
		}

		public void NextLvl() {
			currentLvl++;
		}

		public string GetFirstStarLetter(){
			if (starLetters.Count > 0) {
				string letter = starLetters [0];
				starLetters.RemoveAt(0);
				return letter;
			}
			return "";

		}
	}
}