using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using Assets.Scripts.Metrics.Model;
using SimpleJSON;
using Assets.Scripts.Common;

public class PlagasActivityModel : LevelModel {
	private const int TILES = 36;
	//time is in seconds
	public const int MOLE_TIME = 9, VEGETABLE_TO_MOLE = 2, VEGETABLES_IN_START = 2, MOLES_TO_NEXT_LEVEL = 5;
	private List<PlagaTile> tiles;
	private int smackedMoles, lives;

	private int currentLvl;
	List<PlagasLevel> lvls;

	public PlagasActivityModel() {
		currentLvl = 0;
		smackedMoles = 0;
		lives = 3;
		StartLevels();
		ResetTiles();
		MetricsController.GetController().GameStart();
	}

	public bool GameEnded(){
		return currentLvl == lvls.Count;
	}

	public bool NoMoreLives() {
		return lives == 0;
	}

	public int GetFreeSlot() {
		Randomizer tileRandomizer = Randomizer.New(tiles.Count - 1);
		bool valid = false;

		int nextSpot = -1;
		while(!valid){
			nextSpot = tileRandomizer.Next();
			if(IsSpotFree(nextSpot)) valid = true;
		}
		return nextSpot;
	}

	public int GetLives() {
		return lives;
	}

	public bool IsLevelEnded() {
		return lvls[currentLvl].HasTime() ? smackedMoles == MOLES_TO_NEXT_LEVEL : smackedMoles == lvls[currentLvl].MoleQuantity();
	}

	public bool IsSpotFree(int spot) {
		return tiles[spot].GetState() == PlagaState.FREE;
	}

	public void DecreaseTimer() {
		foreach(PlagaTile tile in tiles) {
			tile.DecreaseTimer();
		}
	}

	public void SetTimerTile(int freeSlot, int randomSpawn) {
		tiles[freeSlot].SetTimeToAppear(randomSpawn);
	}

	void ResetTiles() {
		tiles = new List<PlagaTile>();
		for(int i = 0; i < TILES; i++) {
			tiles.Add(new PlagaTile());
		}
	}

	void StartLevels() {
		lvls = new List<PlagasLevel>();
		JSONArray lvlsJson = JSON.Parse(Resources.Load<TextAsset>("Jsons/PlagasActivity/levels").text).AsObject["levels"].AsArray;
		foreach(JSONNode lvlJson in lvlsJson) {
			lvls.Add(new PlagasLevel(lvlJson.AsObject));
		}
	}

	public List<PlagaTile> GetTiles() {
		return tiles;
	}

	public PlagasLevel CurrentLvl() {
		return lvls[currentLvl];
	}

	public bool HasTime() {
		return CurrentLvl().HasTime();
	}

	public void NextLvl(){
		currentLvl++;
		smackedMoles = 0;
	}

	public void Correct() {
		LogAnswer(true);

		smackedMoles++;
	}

	public void OneLessLife(){
		lives--;
	}

	public void Wrong(){
		LogAnswer(false);
	}

	public int GetRow(string number) {
		List<string> numbers = new List<string>{ "6", "5", "4", "3", "2", "1" };
		return numbers.IndexOf(number);
	}

	public int GetColumn(string letter) {
		List<string> letters = new List<string>{ "A", "B", "C", "D", "E", "F" };
		return letters.IndexOf(letter);
	}

	public int GetSlot(int row, int column) {
		return row * 6 + column;
	}

	public bool IsCorrectTime(int row, int column) {
		int result = GetSlot(row, column);

		return tiles[result].IsCorrect();
	}
}
