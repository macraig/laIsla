using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Games;
using UnityEngine;
using Assets.Scripts.Metrics.Model;
using SimpleJSON;

using Assets.Scripts.Common;

public class TravesiaActivityModel : LevelModel {
	public const int SHIP_QUANTITY = 6, GRID_ROWS = 5, GRID_COLS = 8, PROVISION_SUM = 3;
	public static List<string> letters = new List<string>{ "A", "B", "C", "D", "E", "F", "G", "H" }, numbers = new List<string>{ "5", "4", "3", "2", "1" };
	private List<List<TravesiaEvent>> rows;
	private List<TravesiaEvent> doneEvents;

	private int sendRow, sendCol;

	public TravesiaActivityModel() {
		doneEvents = new List<TravesiaEvent>();
		EmptyRows();
		MetricsController.GetController().GameStart();
	}

	public bool GameEnded(){
		return doneEvents.Count >= SHIP_QUANTITY;
	}

	void EmptyRows() {
		rows = new List<List<TravesiaEvent>>();
		for(int i = 0; i < GRID_ROWS; i++) {
			rows.Add(new List<TravesiaEvent>());
		}
	}

	public bool IsRowEmpty(int row) {
		return rows[row].Count == 0;
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
		return numbers.IndexOf(number);
	}

	public int GetColumn(string letter) {
		return letters.IndexOf(letter);
	}

	public string GetRowString(int row) {
		return numbers[row];
	}

	public string GetColumnString(int col) {
		return letters[col];
	}

	public int GetSlot(int row, int column) {
		return row * GRID_COLS + column;
	}

	public TravesiaEvent GetEvent(int row, int col) {
		return GetRows()[row].Find(t => t.col == col && t.row == row);
	}

	public void AdvanceRow(List<TravesiaEvent> row) {
		row.ForEach(slot => slot.AdvanceOne());
	}

	public bool ExecuteAction(int row, int col, TravesiaAction action) {
		if(action == TravesiaAction.SEND){
			if((col != 0 && col != GRID_COLS - 1) || rows[row].Count != 0)
				return false;

			rows[row].Add(new TravesiaEvent(TravesiaEventState.SHIP, row, col == 0 ? GRID_COLS - 1 : 0, row == sendRow && col == sendCol));
		} else {
			TravesiaEvent spotEvent = rows[row].Find(t => t.col == col && t.row == row);

			if(spotEvent == null) return false;

			return spotEvent.ExecuteAction(spotEvent, action);
		}

		return true;
	}

	public void CleanRow(int rowNumber) {
		List<TravesiaEvent> row = rows[rowNumber];

		if(row.Count == 0) return;
		//ship is always first.
		TravesiaEvent ship = row[0];

		if(row.Count == 1 && (ship.col == 0 || ship.col == GRID_COLS - 1)) return;

		//check if provisions are done.
		if((ship.GetState() == TravesiaEventState.SHIP || ship.GetState() == TravesiaEventState.WRECKED_SHIP) && ship.GetProvisions() == 0) {
			rows[rowNumber] = new List<TravesiaEvent>{ ship.SetState(TravesiaEventState.SUNK_SHIP) };

		}
		if(row.Count == 2){
			//check if monster sunk the ship.
			TravesiaEvent monster = row[1];
			if(monster.col == ship.col){
				rows[rowNumber] = new List<TravesiaEvent> { new TravesiaEvent(TravesiaEventState.SUNK_SHIP, ship.row, ship.col) };
			}
		}
		if(row.Find(e => e.GetState() == TravesiaEventState.REMOVE_SUNK_SHIP) != null){
			//check if sunk ship has been around for one turn.
			rows[rowNumber] = new List<TravesiaEvent>();
		}
		if(row.Find(e => e.GetState() == TravesiaEventState.DEAD_MONSTER) != null){
			rows[rowNumber] = row.GetRange(0, 1);
		}
	}

	public TravesiaEvent CheckForRandomEvents() {
		TravesiaEvent newEvent = null;
		if(Randomizer.RandomBoolean()){
			int row = Randomizer.New(GRID_ROWS - 1).Next();
			//this means it only has a ship
			if(rows[row].Count == 1){
				TravesiaEvent ship = rows[row][0];
				bool wreckShip = Randomizer.RandomBoolean();

				if(wreckShip && ship.GetState() == TravesiaEventState.WRECKED_SHIP) wreckShip = false;
				if(!wreckShip && !CanIntroduceMonster(ship)) wreckShip = true;

				if(ship.GetState() == TravesiaEventState.SHIP || ship.GetState() == TravesiaEventState.WRECKED_SHIP){
					if(wreckShip) {
						newEvent = ship.SetState(TravesiaEventState.WRECKED_SHIP);
					}
					else if(CanIntroduceMonster(ship)) {
						newEvent = NewMonsterForShip(ship);
						rows[row].Add(newEvent);
					}
				}
			}
		}
		return newEvent;
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

	public TravesiaEvent GetAndRemoveDoneShipsFromRow(int rowNumber) {
		List<TravesiaEvent> row = rows[rowNumber];

		if(row.Count == 0) return null;

		if(row[0].col == 0 || row[0].col == GRID_COLS - 1){
			TravesiaEvent ship = row[0];
			doneEvents.Add(ship);
			rows[rowNumber] = new List<TravesiaEvent>();
			return ship;
		}

		return null;
	}

	//FOR LOG:

	public List<TravesiaEvent> GetDoneEvents(){
		return doneEvents;
	}

	public List<TravesiaEvent> GetOnGoingShipEvents(){
		List<TravesiaEvent> result = new List<TravesiaEvent>();
		foreach(List<TravesiaEvent> row in rows) {
			TravesiaEvent shipEvent = row.Find(e => e.GetState() == TravesiaEventState.SHIP || e.GetState() == TravesiaEventState.WRECKED_SHIP);
			if(shipEvent != null)
				result.Add(shipEvent);
		}
		return result;
	}

	//CONSIGNA:

	public void NewSend() {
		Randomizer rowRandomizer = Randomizer.New(GRID_ROWS - 1);
		for(int i = 0; i < GRID_ROWS; i++) {
			int row = rowRandomizer.Next();
			if(rows[row].Count == 0) {
				sendRow = row;
				break;
			}
		}
		sendCol = Randomizer.RandomBoolean() ? 0 : (GRID_COLS - 1);

		if(rows[sendRow].Count != 0) {
			sendRow = -1;
			sendCol = -1;
		}
	}

	public void CheckSend(){
		if(sendRow == -1 || rows[sendRow].Count != 0)
			NewSend();
	}

	public int SendRow() {
		return sendRow;
	}
	public int SendCol() {
		return sendCol;
	}
}