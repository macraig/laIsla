using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using Assets.Scripts.Metrics.Model;
using SimpleJSON;
using Assets.Scripts.Common;

public class TravesiaActivityModel : LevelModel {
	public const int TILES = 36, SHIP_QUANTITY = 6, GRID_ROWS = 6, GRID_COLS = 9, PROVISION_SUM = 3;
	private List<List<TravesiaEvent>> rows;
	private int arrivedShips;

	private int sendRow, sendCol;

	public TravesiaActivityModel() {
		arrivedShips = 0;
		EmptyRows();
		MetricsController.GetController().GameStart();
	}

	public bool GameEnded(){
		return arrivedShips == SHIP_QUANTITY;
	}

	void EmptyRows() {
		rows = new List<List<TravesiaEvent>>();
		for(int i = 0; i < GRID_ROWS; i++) {
			rows.Add(new List<TravesiaEvent>());
		}
	}

	public List<List<TravesiaEvent>> GetRows() {
		return rows;
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
		return row * GRID_ROWS + column;
	}

	public void AdvanceOne() {
		rows.ForEach(row => row.ForEach(slot => slot.AdvanceOne()));
	}

	public bool ExecuteAction(int row, int col, TravesiaAction action) {
		if(action == TravesiaAction.SEND){
			if((col != 0 && col != GRID_COLS - 1) || rows[row].Count != 0)
				return false;

			rows[row].Add(new TravesiaEvent(TravesiaEventState.SHIP, row, col, row == sendRow && col == sendCol));
		} else {
			TravesiaEvent spotEvent = rows[row].Find(t => t.col == col && t.row == row);

			if(spotEvent == null) return false;

			return spotEvent.ExecuteAction(spotEvent, action);
		}

		return true;
	}

	public void CleanRows() {
		for(int i = 0; i < rows.Count; i++) {
			List<TravesiaEvent> row = rows[i];

			if(row.Count == 0) continue;
			//ship is always first.
			TravesiaEvent ship = row[0];

			if(row.Count == 1 && (ship.col == 0 || ship.col == GRID_COLS - 1)) continue;

			//check if provisions are done.
			if((ship.GetState() == TravesiaEventState.SHIP || ship.GetState() == TravesiaEventState.WRECKED_SHIP) && ship.GetProvisions() == 0) {
				rows[i] = new List<TravesiaEvent>();
			}
			if(row.Count == 2){
				//check if monster sunk the ship.
				TravesiaEvent monster = row[1];
				if(monster.col == ship.col){
					rows[i] = new List<TravesiaEvent> { new TravesiaEvent(TravesiaEventState.SUNK_SHIP, ship.row, ship.col) };
				}
			}
			if(row.Find(e => e.GetState() == TravesiaEventState.REMOVE_SUNK_SHIP) != null){
				//check if sunk ship has been around for one turn.
				rows[i] = new List<TravesiaEvent>();
			}
			if(row.Find(e => e.GetState() == TravesiaEventState.DEAD_MONSTER) != null){
				rows[i] = row.GetRange(0, 1);
			}
		}
	}

	public void CheckForRandomEvents() {
		if(Randomizer.RandomBoolean()){
			int row = Randomizer.New(GRID_ROWS - 1).Next();
			//this means it only has a ship
			if(rows[row].Count == 1){
				TravesiaEvent ship = rows[row][0];
				bool wreckShip = Randomizer.RandomBoolean();

				if(wreckShip && ship.GetState() == TravesiaEventState.WRECKED_SHIP) wreckShip = false;
				if(!wreckShip && !CanIntroduceMonster(ship)) wreckShip = true;

				if(ship.GetState() == TravesiaEventState.SHIP || ship.GetState() == TravesiaEventState.WRECKED_SHIP){
					if(wreckShip)
						ship.SetState(TravesiaEventState.WRECKED_SHIP);
					else if(CanIntroduceMonster(ship)) {
						rows[row].Add(NewMonsterForShip(ship));
					}
				}
			}
		}
	}

	TravesiaEvent NewMonsterForShip(TravesiaEvent ship) {
		int monsterCol;
		if(ship.isGoingLeft){
			monsterCol = ship.col == 2 ? 1 : Randomizer.New(ship.col - 1, 1).Next();
		} else {
			monsterCol = ship.col == GRID_COLS - 3 ? GRID_COLS - 2 : Randomizer.New(GRID_COLS - 2, ship.col + 1).Next();
		}

		return new TravesiaEvent(TravesiaEventState.MONSTER, ship.row, monsterCol);
	}

	bool CanIntroduceMonster(TravesiaEvent ship) {
		if(ship.isGoingLeft && ship.col >= 2) return true;
		if(!ship.isGoingLeft && ship.col <= GRID_COLS - 3) return true;

		return false;
	}
}
