using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using Assets.Scripts.Metrics.Model;
using SimpleJSON;

namespace Assets.Scripts.Games.RompecabezasActivity {
	public class RompecabezasActivityModel : LevelModel {
		//time is in seconds
		public const int START_TIME = 60, CORRECT_SCENE_TIME = 20;
		private int timer;
		private bool withTime;

		private int currentLvl;
		List<RompecabezasLevel> lvls;

		public RompecabezasActivityModel() {
			currentLvl = 0;
			timer = START_TIME;
			withTime = false;
			StartLevels();
			MetricsController.GetController().GameStart();
		}

		public bool GameEnded(){
			return currentLvl == lvls.Count;
		}

		void StartLevels(bool withTime = false) {
			lvls = new List<RompecabezasLevel>();
			JSONArray lvlsJson = JSON.Parse(Resources.Load<TextAsset>("Jsons/RompecabezasActivity/levels").text).AsObject["levels"].AsArray;
			foreach(JSONNode lvlJson in lvlsJson) {
				lvls.Add(new RompecabezasLevel(lvlJson.AsObject, withTime));
			}
		}

		public RompecabezasLevel CurrentLvl() {
			return lvls[currentLvl];
		}

		public bool HasTime() {
			return withTime;
		}

		public void NextLvl(){
			currentLvl++;

			if(currentLvl == lvls.Count){
				withTime = true;
				currentLvl = 0;
				StartLevels(true);
			}
		}

		public void WithTime(){
			withTime = true;
		}

		public void Correct() {
			LogAnswer(true);
		}

		public void Wrong(){
			LogAnswer(false);
		}

		public void DecreaseTimer() {
			if(timer > 0) timer--;
		}

		public bool IsTimerDone(){
			return timer == 0;
		}

		public void CorrectTimer() {
			timer += CORRECT_SCENE_TIME;
		}

		public int GetTimer() {
			return timer;
		}
	}
}