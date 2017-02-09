using System;
using Assets.Scripts.Metrics;
using Assets.Scripts.Common;

public class TravesiaEvent {
	private TravesiaEventState state;
	//SHIP stuff. Object number is used to save the ship color and the monster sprite number.
	private int provisions, objectNumber;

	public bool isGoingLeft, isShipCorrect;
	public int row, col;

	public TravesiaEvent(TravesiaEventState initialState, int row, int col, bool isShipCorrect = false){
		state = initialState;
		provisions = 0;
		if(initialState == TravesiaEventState.SHIP) {
			provisions = Randomizer.New(7, 3).Next();
			objectNumber = Randomizer.New(2).Next();
		}
		if(initialState == TravesiaEventState.MONSTER) {
			objectNumber = Randomizer.New(1).Next();
		}

		this.isGoingLeft = col != 0;
		this.row = row;
		this.col = col;
		this.isShipCorrect = isShipCorrect;
	}

	public void DecreaseTimer(){
		if(provisions != 0) provisions--;
	}

	public TravesiaEventState GetState() {
		return state;
	}

	public int GetProvisions(){
		return provisions;
	}

	public TravesiaEvent SetState(TravesiaEventState state){
		this.state = state;
		return this;
	}

	public int GetObjectNumber(){
		return objectNumber;
	}

	public void AdvanceOne() {
		if(provisions > 0) provisions--;
		if(state == TravesiaEventState.SHIP) {
			if(isGoingLeft) col--;
			else col++;
		}
		if(state == TravesiaEventState.SUNK_SHIP)
			state = TravesiaEventState.REMOVE_SUNK_SHIP;
	}

	public bool ExecuteAction(TravesiaEvent spotEvent, TravesiaAction action) {
		if(action == TravesiaAction.REPAIR && state == TravesiaEventState.WRECKED_SHIP) {
			state = TravesiaEventState.SHIP;
		} else if(action == TravesiaAction.ATTACK && state == TravesiaEventState.MONSTER) {
			state = TravesiaEventState.DEAD_MONSTER;
		} else if(action == TravesiaAction.ATTACK && (state == TravesiaEventState.SHIP || state == TravesiaEventState.WRECKED_SHIP)){
			state = TravesiaEventState.SUNK_SHIP;
		} else if(action == TravesiaAction.PROVISION && (state == TravesiaEventState.SHIP || state == TravesiaEventState.WRECKED_SHIP)) {
			provisions += TravesiaActivityModel.PROVISION_SUM;
		} else
			return false;
		return true;
	}
}