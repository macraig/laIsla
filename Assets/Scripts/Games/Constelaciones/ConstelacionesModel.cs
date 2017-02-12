using System;
using Assets.Scripts.Metrics.Model;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Assets.Scripts.Games.Constelaciones {
	public class ConstelacionesModel : LevelModel {
		private int currentLvl;
		List<ConstelacionesLevel> lvls;

		public ConstelacionesModel() {
			currentLvl = 0;
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
		}

		public ConstelacionesLevel CurrentLvl() {
			return lvls[currentLvl];
		}
	}
}