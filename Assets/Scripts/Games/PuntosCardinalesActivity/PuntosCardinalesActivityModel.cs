﻿using System;
using Assets.Scripts.Metrics.Model;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Games.PuntosCardinalesActivity {
	public class PuntosCardinalesActivityModel : LevelModel {
		public static Building STREET = Building.B("street","","", false, true);
		public static int SIMPLE_BUILDINGS = 14;
		public static int ROUNDS = 9;
		private int currentLvl;

		private Dictionary<string, Sprite> buildingSprites, referencesSprites;
		private Dictionary<string, AudioClip> audios;
		private List<List<Building>> grid;
		private List<Building> simpleBuildings, options;
		private string[] names,textNamesStart,textNamesEnd;
		private List<PuntosCardinalesLevel> lvls;
		private bool answerIsCorrect;

		public List<Building> GetGrid() {
			List<Building> result = new List<Building>();
			foreach(List<Building> l in grid) {
				result.AddRange(l);
			}
			return result;
		}

		public List<AudioClip> GetAudios() {
			return lvls[currentLvl].GetAudios();
		}

		public string GetText() {
			return lvls[currentLvl].GetText(grid);
		}

		public bool IsCorrect ()
		{
			return answerIsCorrect;
		}

		public void SetCorrect (bool answer)
		{
			answerIsCorrect = answer;
		}

		public bool IsCorrectDragger(Sprite dragger) {
			return lvls[currentLvl].IsCorrectDragger(dragger, buildingSprites);
		}

		public bool IsCorrectSlot(int row, int column) {
			return lvls[currentLvl].IsCorrectSlot(row, column);
		}

		//randomizo 5 cualquiera 
		public List<Building> GetChoices() {
			return lvls [currentLvl].GetChoices (simpleBuildings);
		}

		public Sprite GetSprite(Building b){
			return b.GetSprite(buildingSprites);
		}

		public Sprite GetReferenceSprite(Building b){
			return b.GetSprite(referencesSprites);
		}

		public string GetBuildingTextName (Building b)
		{
			return b.GetTextNameStart().Substring(3).ToUpper();
		}

		public void NextLvl(){
			currentLvl++;
		}

		public bool GameEnded(){
			return currentLvl == ROUNDS;
		}

		public int CurrentLvl(){
			return currentLvl;
		}

		public PuntosCardinalesActivityModel() {
			currentLvl = 0;
			//nombres de los edificios en orden de sprites.
			InitNames();
			//inicia la grilla con las calles y la laguna.
			InitGrid();
			//inicia el sprite de cada edificio con su nombre.
			InitSprites();
			//inicia los audios con sus nombres consignas.
			InitAudios();
			//inicia los objetos de los edificios simples.
			InitSimpleBuildings();

			//randomiza las respuestas correctas.
			RandomizeOptions();

			//mete la plaza y el establo en la grilla.
			SetDoubleBuildingsInGrid();
			//mete los 5 random en la grilla y los borra de simpleBuildings.
			SetSimpleBuildingsInGrid();

			MetricsController.GetController().GameStart();
		}

		//create each level, modifying the grid to create the next level.
		public void CreateLevels() {
			lvls = new List<PuntosCardinalesLevel>();
			foreach(Building o in options) {
				lvls.Add(PuntosCardinalesLevel.CreateLevel(grid, o, audios));
			}
		}

		void RandomizeOptions() {
			options = new List<Building>();
			Randomizer r = Randomizer.New(simpleBuildings.Count - 1);

			while(options.Count < ROUNDS){
				options.Add(simpleBuildings[r.Next()]);
			}
		}

		Building GetSimpleBuilding() {
			Randomizer r = Randomizer.New(simpleBuildings.Count - 1);
			Building b = null;
			while(b == null || options.Contains(b)){
				b = simpleBuildings[r.Next()];
			}

			simpleBuildings.Remove(b);
			return b;
		}

		// Cableado siguiendo lógica pedida
		void SetSimpleBuildingsInGrid() {
			Building a = GetSimpleBuilding();
			bool up = Randomizer.RandomBoolean();
			Randomizer r = Randomizer.New(up ? 5 : 3, up ? 2 : 0);
			//primero: el que va con la plaza o el establo.
			while(true){
				int spot = r.Next();

				if(grid[up ? 0 : 5][spot] == null){
					grid[up ? 0 : 5][spot] = a;
					break;
				}
			}

			//segundo: los dos de la izquierda.
			r = Randomizer.New(3);

			grid[r.Next()][0] = GetSimpleBuilding();
			grid[r.Next()][0] = GetSimpleBuilding();

			//tercero: los dos de la derecha.
			r = Randomizer.New(5, 2);

			grid[r.Next()][5] = GetSimpleBuilding();
			grid[r.Next()][5] = GetSimpleBuilding();
		}

		void SetDoubleBuildingsInGrid() {
			bool doubleRandom = Randomizer.RandomBoolean();

			Building upLeft = doubleRandom ? Building.B("establo","el establo","del establo", true, false, true) : Building.B("plaza","la plaza","de la plaza", true, false, true);
			Building upRight = doubleRandom ? Building.B("establo","el establo","del establo", true) : Building.B("plaza","la plaza","de la plaza", true);
			Building downLeft = doubleRandom ? Building.B("plaza","la plaza","de la plaza", true, false, true) : Building.B("establo","el establo","del establo", true, false, true);
			Building downRight = doubleRandom ? Building.B("plaza","la plaza","de la plaza", true) : Building.B("establo","el establo","del establo", true);

			bool middleRandom = Randomizer.RandomBoolean();
			bool left = Randomizer.RandomBoolean();

			if(middleRandom){
				grid[0][3] = upLeft;
				grid[0][4] = upRight;

				grid[5][left ? 0 : 2] = downLeft;
				grid[5][left ? 1 : 3] = downRight;
			} else {
				grid[0][left ? 2 : 4] = upLeft;
				grid[0][left ? 3 : 5] = upRight;

				grid[5][1] = downLeft;
				grid[5][2] = downRight;
			}
		}

		public void Correct() {
			LogAnswer(true);
		}

		public void Wrong(){
			LogAnswer(false);
		}

		void InitSimpleBuildings() {
			simpleBuildings = new List<Building>();
			for(int i = 0; i < SIMPLE_BUILDINGS; i++) {
				simpleBuildings.Add(Building.B(names[i],textNamesStart[i],textNamesEnd[i]));
			}
		}

		void InitAudios() {
			audios = new Dictionary<string, AudioClip>();

			foreach(AudioClip a in Resources.LoadAll<AudioClip>("Audio/PuntosCardinalesActivity/Edificios")) {
				audios.Add(a.name, a);
			}
			foreach(AudioClip a in Resources.LoadAll<AudioClip>("Audio/PuntosCardinalesActivity/EdificiosFinal")) {
				audios.Add(a.name, a);
			}
			foreach(AudioClip a in Resources.LoadAll<AudioClip>("Audio/PuntosCardinalesActivity/Consignas")) {
				audios.Add(a.name, a);
			}
		}

		void InitNames() {
			names = new string[] {
				"verduleria",
				"pescaderia",
				"sastreria",
				"armeria",
				"carpinteria",
				"alfareria",
				"herreria",
				"floreria",
				"biblioteca",
				"cuartel",
				"carniceria",
				"torre",
				"casa",
				"molino",
				"plazaLeft",
				"plazaRight",
				"establoLeft",
				"establoRight",
				"lagunaLeft",
				"lagunaRight"
			};

			textNamesStart = new string[] {
				"la verdulería",
				"la pescadería",
				"la sastrería",
				"la armería",
				"la carpintería",
				"la alfarería",
				"la herrería",
				"la florería",
				"la biblioteca",
				"el cuartel",
				"la carnicería",
				"la torre",
				"la casa",
				"el molino",
				"la plaza",
				"la plaza",
				"el establo",
				"el establo",
				"la laguna",
				"la laguna"
			};

			textNamesEnd = new string[] {
				"de la verdulería",
				"de la pescadería",
				"de la sastrería",
				"de la armería",
				"de la carpintería",
				"de la alfarería",
				"de la herrería",
				"de la florería",
				"de la biblioteca",
				"del cuartel",
				"de la carnicería",
				"de la torre",
				"de la casa",
				"del molino",
				"de la plaza",
				"de la plaza",
				"del establo",
				"del establo",
				"de la laguna",
				"de la laguna"
			};


		}

		void InitSprites() {
			buildingSprites = new Dictionary<string, Sprite>();
			Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/PuntosCardinalesActivity/edificios");

			for(int i = 0; i < names.Length; i++) {
				buildingSprites.Add(names[i], sprites[i]);
			}

			referencesSprites = new Dictionary<string, Sprite>();
			Sprite[] refSprites = Resources.LoadAll<Sprite>("Sprites/PuntosCardinalesActivity/referencias");

			for(int j = 0; j < SIMPLE_BUILDINGS; j++) {
				referencesSprites.Add(names[j], refSprites[j]);
			}

		}

		void InitGrid() {
			grid = new List<List<Building>>();
			grid.Add(new List<Building>{ null, STREET, null, null, null, null });
			grid.Add(new List<Building>{ null, STREET, STREET, STREET, STREET, STREET });
			grid.Add(new List<Building>{ null, STREET, null, null, STREET, null });
			grid.Add(new List<Building>{ null, STREET, Building.B("laguna","la laguna","de la laguna", true, false, true), Building.B("laguna","la laguna","de la laguna", true), STREET, null });
			grid.Add(new List<Building>{ STREET, STREET, STREET, STREET, STREET, null });
			grid.Add(new List<Building>{ null, null, null, null, STREET, null });
		}
	}
}