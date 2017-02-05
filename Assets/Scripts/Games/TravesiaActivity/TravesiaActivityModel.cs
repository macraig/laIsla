using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using Assets.Scripts.Metrics.Model;
using SimpleJSON;
using Assets.Scripts.Common;

public class TravesiaActivityModel : LevelModel {
	private const int TILES = 36, SHIP_QUANTITY = 6;
	//time is in seconds
	private List<TravesiaTile> tiles;
	private int arrivedShips;

	public TravesiaActivityModel() {
		arrivedShips = 0;
		ResetTiles();
		MetricsController.GetController().GameStart();
	}

	public bool GameEnded(){
		return arrivedShips == SHIP_QUANTITY;
	}

	void ResetTiles() {
		tiles = new List<TravesiaTile>();
		for(int i = 0; i < TILES; i++) {
			tiles.Add(new TravesiaTile());
		}
	}

	public List<TravesiaTile> GetTiles() {
		return tiles;
	}

	public void Correct() {
		LogAnswer(true);
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
}
